using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using k.libary;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Newtonsoft.Json;
using System.Deployment.Application;

namespace AutoPos
{

    public partial class frmAutoPos : frmSub
    {
        CultureInfo us = CultureInfo.GetCultureInfo("en-US");

        frmUpdateProgress frmpro = new frmUpdateProgress();
        string smsUpdate = "";

        string StrConn;
        string StrConnSup;
        string Whcode;

        List<POS> ListPOS = new List<POS>();
        //List<POSPIS> ListPI = new List<POSPIS>();
        //List<POSPTPRS> ListPTPR = new List<POSPTPRS>();


        CMDDataContext cmd = new CMDDataContext();
        POSULDataContext sup = new POSULDataContext();

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                var clickOnceHelper = new ClickOnceHelper(Globals.PublisherName, Globals.ProductName);
                clickOnceHelper.UpdateUninstallParameters();
                clickOnceHelper.AddShortcutToStartup();

                smsUpdate = "ปรับปรุง:" + Environment.NewLine;
                //"1. เพิ่มรายงานสินค้า พร้อมราคาขาย , ราคาโปรโมชั่น" + Environment.NewLine;    

                //ถ้าเป็นการรันโปรแกรมจากการติดตั้งด้วย ClickOnce 
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    //เรียกเมธอด UpdateApplication
                    UpdateApplication();

                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            }
            base.OnLoad(e);
        }

        public frmAutoPos()
        {
            InitializeComponent();
            StrConn = "Data Source=(local)\\sqlexpress;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            StrConnSup = "Data Source=(local)\\sqlexpress;Initial Catalog=dbBeautycommsupport;User ID=sa;password=0000";
            ////StrConn = "Data Source=AYUD2.DYNDNS.info,1401;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            ////StrConnSup = "Data Source=AYUD2.DYNDNS.info,1401;Initial Catalog=dbBeautycommsupport;User ID=sa;password=0000";

            Whcode = "";
            //StrConn = "Data Source=.;Initial Catalog=CMD-FX;User ID=sa;password=1Q2w3e4r@";
            //StrConnSup = "Data Source=.;Initial Catalog=dbBeautycommsupport;User ID=sa;password=1Q2w3e4r@";
            //Whcode = "1006";

            getText();
        }

        public frmAutoPos(string _strconn, string _strconnSup, string _whcode)
        {
            InitializeComponent();
            StrConn = _strconn;
            StrConnSup = _strconnSup;
            Whcode = _whcode;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            //getTime();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //setPath();
           
            cmd.Connection.ConnectionString = StrConn;
            sup.Connection.ConnectionString = StrConnSup;

            if(Whcode == "")
            {
                setWhcode();
            }
            label1.Text = Whcode;
            Displaynotify();
            this.ShowInTaskbar = false;
            tm.Start();

        }

        private void autoSend()
        {
           
            try
            {
                ListPOS.Clear();

                if (getABBNO() == true)
                {
                    using (var client = new HttpClient())
                    {
                        if (ListPOS.Count > 0)
                        {
                            int sta = 0;
                            string sms = "";
                            client.BaseAddress = new Uri("http://5cosmeda.homeunix.com:89/ApiFromPOS/");
                            //client.BaseAddress = new Uri("http://192.168.10.202:89/ApiFromPOS/");
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var json = JsonConvert.SerializeObject(ListPOS);

                            //---------------------------------------------------------------------//
                            var list = (from pp in ListPOS
                                        select pp).FirstOrDefault();

                            string abn = list.POSPT.ABBNO;
                            //---------------------------------------------------------------------//

                            JSONSTRING ss = new JSONSTRING();
                            ss.DATAJSON = json;

                            var response = client.PostAsJsonAsync("api/POS/InsertBill", ss).Result;
                            var details = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                            if ((int)response.StatusCode == 200)
                            {
                                sta = Convert.ToInt32(details["StatusCode"]);
                                sms = details["Messages"].ToString();

                                if (sta == 1)
                                {
                                    upPosUL(abn);
                                    //upPosUL();
                                    uplog(Whcode, "3" + sms);
                                }
                                else if (sta == 2)
                                {
                                    if (sms.Substring(1,9) == "Violation" || sms == "Object reference not set to an instance of an object.")
                                    {
                                        
                                        uplog(Whcode, "5" + sms);
                                    }
                                    else
                                    {
                                        upPosUL();
                                        uplog(Whcode, "4" + "SUCCESS");
                                    }
                                }
                                else
                                {
                                    uplog(Whcode, "2" + sms);
                                }
                            }
                            else
                            {
                                uplog(Whcode, "1"+response.StatusCode.ToString());

                            }
                        }
                        else
                        {
                            uplog(Whcode, "No bill");
                        }
                        

                    }
                }
                else
                {
                    uplog(Whcode, "No bill");
                }
            }
            catch(Exception ex)
            {
                uplog(Whcode, "4" + ex.Message);
            }
            
        }

        private bool getABBNO()
        {
            bool bl = false;
        

            try
            {
                
                var result = cmd.PV_POS_ULs.ToList();

                foreach (var item in result)
                {
                    POS_UL ul = new POS_UL();
                    ul.WH_ID = item.WH_ID;
                    ul.ABBNO = item.ABBNO;
                    ul.PTDATE = item.PTDATE;
                    ul.WORKDATE = item.WORKDATE;
                    ul.TMCODE = item.TMCODE;
                    ul.UFLAG = "N";

                    sup.POS_ULs.InsertOnSubmit(ul);
                    sup.SubmitChanges();
                }


                //var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N").ToList();
                var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N").Take(1).ToList();

                foreach (var item in bill_rs)
                {
                    getList(item.WH_ID, item.TMCODE, item.WORKDATE, item.ABBNO);
                }

                if (bill_rs.Count > 0 )
                {
                    bl = true;
                }

               
            }
            catch(Exception ex)
            {
                uplog(Whcode, "5" + ex.Message);
                //MessageBox.Show(ex.Message);
                bl = false;
            }
            
            return bl;
        }

        private void getList(int _wh_id,string _tmcode,DateTime _workdate,string _abbno)
        {

            var sql_pt = cmd.POS_PTs.Where(s => s.WH_ID == _wh_id && s.WORKDATE == _workdate && s.TMCODE == _tmcode && s.ABBNO == _abbno).FirstOrDefault();

            var sql_pi = cmd.POS_PIs.Where(s => s.WH_ID == _wh_id && s.WORKDATE == _workdate && s.TMCODE == _tmcode && s.ABBNO == _abbno).ToList();

            var sql_pr = cmd.POS_PT_PRs.Where(s => s.WH_ID == _wh_id && s.WORKDATE == _workdate && s.TMCODE == _tmcode && s.ABBNO == _abbno).ToList();

            List<POSPIS> ListPI = new List<POSPIS>();
            List<POSPTPRS> ListPTPR = new List<POSPTPRS>();

            POSPTS pt = new POSPTS
                {
                    WH_ID = sql_pt.WH_ID,
                    TMCODE = sql_pt.TMCODE,
                    ABBNO = sql_pt.ABBNO,
                    PTDATE = sql_pt.PTDATE,
                    PTTIME = sql_pt.PTTIME,
                    SHIFTNO = sql_pt.SHIFTNO,
                    PTSTATUS = sql_pt.PTSTATUS,
                    OCSTATUS = sql_pt.OCSTATUS,
                    WORKDATE = sql_pt.WORKDATE,
                    ST_ID_LOG = sql_pt.ST_ID_LOG,
                    ST_ID = sql_pt.ST_ID,
                    IV_DOCNO = sql_pt.IV_DOCNO,
                    CN_DOCNO = sql_pt.CN_DOCNO,
                    REFNO = sql_pt.REFNO,
                    CT_ID = sql_pt.CT_ID,
                    CT_BILL_TO = sql_pt.CT_BILL_TO,
                    CT_BILL_ADDR1 = sql_pt.CT_BILL_ADDR1,
                    CT_BILL_ADDR2 = sql_pt.CT_BILL_ADDR2,
                    CT_BILL_ADDR3 = sql_pt.CT_BILL_ADDR3,
                    CT_CARDID = sql_pt.CT_CARDID,
                    CTTYPE = sql_pt.CTTYPE,
                    CTCAT = sql_pt.CTCAT,
                    CO_ID = sql_pt.CO_ID,
                    CO_CARDID = sql_pt.CO_CARDID,
                    POINT = sql_pt.POINT,
                    POINT_TOTAL = sql_pt.POINT_TOTAL,
                    SDPCODE = sql_pt.SDPCODE,
                    PRCODE = sql_pt.PRCODE,
                    REMARK = sql_pt.REMARK,
                    VATTYPE = sql_pt.VATTYPE,
                    VATRATE = sql_pt.VATRATE,
                    QTY = sql_pt.QTY,
                    SUMMARY = sql_pt.SUMMARY,
                    ITEMDISCOUNT = sql_pt.ITEMDISCOUNT,
                    SUBTOTAL = sql_pt.SUBTOTAL,
                    SUBTOTAL_NORM = sql_pt.SUBTOTAL_NORM,
                    DISCOUNTSTR = sql_pt.DISCOUNTSTR,
                    DISCOUNTAMT = sql_pt.DISCOUNTAMT,
                    DISCOUNTAMT_VAT = sql_pt.DISCOUNTAMT_VAT,
                    TOTAL = sql_pt.TOTAL,
                    TOTAL_VAT = sql_pt.TOTAL_VAT,
                    VATAMT = sql_pt.VATAMT,
                    ROUNDAMT = sql_pt.ROUNDAMT,
                    NET = sql_pt.NET,
                    VAL = sql_pt.VAL,
                    PAY_OTHER = sql_pt.PAY_OTHER,
                    PAY_CASH = sql_pt.PAY_CASH,
                    PAY_CASH_TENDER = sql_pt.PAY_CASH_TENDER,
                    PAY_CARD1 = sql_pt.PAY_CARD1,
                    PAY_CARD_CD_ID1 = sql_pt.PAY_CARD_CD_ID1,
                    PAY_CARD_ID1 = sql_pt.PAY_CARD_ID1,
                    PAY_CARD_NAME1 = sql_pt.PAY_CARD_NAME1,
                    PAY_CARD2 = sql_pt.PAY_CARD2,
                    PAY_CARD_CD_ID2 = sql_pt.PAY_CARD_CD_ID2,
                    PAY_CARD_ID2 = sql_pt.PAY_CARD_ID2,
                    PAY_CARD_NAME2 = sql_pt.PAY_CARD_NAME2,
                    RF_IV_DOCNO = sql_pt.RF_IV_DOCNO,
                    RF_IV_RMVALUE = sql_pt.RF_IV_RMVALUE,
                    RF_RIGHTAMT = sql_pt.RF_RIGHTAMT,
                    RF_DIFAMT = sql_pt.RF_DIFAMT,
                    TAG_F = sql_pt.TAG_F,
                    TAG_S =sql_pt.TAG_S
            };

            foreach(var item in sql_pi)
            {
                POSPIS pis1 = new POSPIS
                {
                    WH_ID = item.WH_ID,
                    TMCODE = item.TMCODE,
                    ABBNO = item.ABBNO,
                    PTDATE = item.PTDATE,
                    ITEMNO = item.ITEMNO,
                    MP_ID = item.MP_ID,
                    BC_ID = item.BC_ID,
                    VATABLE = item.VATABLE,
                    CONTROLSN = item.CONTROLSN,
                    SH_ID = item.SH_ID,
                    ACPRICE = item.ACPRICE,
                    PRICENO = item.PRICENO,
                    DPRICE = item.DPRICE,
                    BPRICE = item.BPRICE,
                    PRICE = item.PRICE,
                    UM_ID = item.UM_ID,
                    UMRATIO = item.UMRATIO,
                    QTY = item.QTY,
                    DISCOUNTSTR = item.DISCOUNTSTR,
                    DISCOUNTAMT = item.DISCOUNTAMT,
                    TOTAL = item.TOTAL,
                    NET = item.NET,
                    VAL = item.VAL,
                    POINT = item.POINT,
                    PRCODE = item.PRCODE,
                    OFFERID = item.OFFERID,
                    ITEMAU = item.ITEMAU,
                    ITEMRF = item.ITEMRF,
                    QTY_RF = item.QTY_RF,
                    PR_TOTAL = item.PR_TOTAL,
                    PR_NET = item.PR_NET,
                    PR_VAL = item.PR_VAL,
                    PIVOIDED = item.PIVOIDED,
                    PISTATUS = item.PISTATUS,
                    WORKDATE = item.WORKDATE,
                    TAG_F = item.TAG_F,
                    TAG_S = item.TAG_S
                };
                ListPI.Add(pis1);
            }


            foreach (var item in sql_pr)
            {
              
               POSPTPRS ptpr = new POSPTPRS
               {
                    WH_ID = item.WH_ID,
                    TMCODE = item.TMCODE,
                    ABBNO = item.ABBNO,
                    PTDATE = item.PTDATE,
                    PRCODE = item.PRCODE,
                    ITEMNO = item.ITEMNO,
                    SORTNO = item.SORTNO,
                    CVC_NO = item.CVC_NO,
                    DVAL = item.DVAL,
                    DTYPE = item.DTYPE,
                    TOTALB4DISC = item.TOTALB4DISC,
                    DISCOUNTAMT = item.DISCOUNTAMT,
                    WORKDATE = item.WORKDATE
                };
                ListPTPR.Add(ptpr);
            }

            string brand = "";
            string fstr;
            fstr = Whcode.Substring(0, 1);

            if(fstr == "1" || fstr == "3")
            {
                brand = "BB";
            }
            else if(fstr == "5")
            {
                brand = "BC";
            }
            else
            {
                brand = "BM";
            }

            POS pos = new POS { POSPT = pt, POSPI = ListPI, POSPTPR = ListPTPR ,CREATEBY = Whcode,ENDDAY="N",BRAND = brand};
            ListPOS.Add(pos);

        }


        protected void Displaynotify()
        {
            try
            {
                ntf.Icon = new System.Drawing.Icon(Path.GetFullPath(@"sync.ico"));
                ntf.Text = "POS SYNC";
                ntf.Visible = true;
                ntf.BalloonTipTitle = "POS SYNC";
                ntf.BalloonTipText = "POS SYNC";
                ntf.ShowBalloonTip(50);
            }
            catch (Exception ex)
            {

            }
        }

        private void ntf_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            ntf.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                ntf.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        private int getTime()
        {
            int res = 0;

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://5cosmeda.homeunix.com:89/ApiFromPOS/");
                //client.BaseAddress = new Uri("http://192.168.10.202:89/ApiFromPOS/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("api/POS/TimeSync?WHCODE="+Whcode).Result;
                var details = JObject.Parse(response.Content.ReadAsStringAsync().Result);

               res= Convert.ToInt32(details["Results"]["WHTIME"]);

            }

            return res;

        }

        private void tm_Tick(object sender, EventArgs e)
        {
            try
            {
                tm.Stop();
                clsXML cs = new clsXML();
                cs.setXML();
                int tt = getTime();
                if (tt > 0)
                {
                    tm.Interval = getTime() * 60000;
                    autoSend();
                }
                else
                {
                    uplog(Whcode, "time = 0");
                }

                tm.Start();
            }
            catch(Exception ex)
            {
                uplog(Whcode, ex.Message);
            }
           
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            tm.Stop();

            autoSend();
            //tm.Interval = getTime() * 60000;
            tm.Start();
        }

        private void upPosUL()
        {
            var data = sup.POS_ULs.Where(s => s.UFLAG == "N").ToList();
            foreach (var item in data)
            {
                var updata = sup.POS_ULs.Where(s => s.WH_ID == item.WH_ID && s.WORKDATE == item.WORKDATE && s.TMCODE == item.TMCODE && s.PTDATE == item.PTDATE && s.ABBNO == item.ABBNO).FirstOrDefault();

                updata.UFLAG = "Y";

                sup.SubmitChanges();
            }

        }

        private void upPosUL(string abbno)
        {
            var data = sup.POS_ULs.Where(s => s.UFLAG == "N" && s.ABBNO == abbno).ToList();
            foreach (var item in data)
            {
                var updata = sup.POS_ULs.Where(s => s.WH_ID == item.WH_ID && s.WORKDATE == item.WORKDATE && s.TMCODE == item.TMCODE && s.PTDATE == item.PTDATE && s.ABBNO == item.ABBNO).FirstOrDefault();

                updata.UFLAG = "Y";

                sup.SubmitChanges();
            }

        }

        private void uplog(string _whcode,string _sms)
        {
           trn_log_pos lp = new trn_log_pos();
            lp.whcode = _whcode;
            lp.workdate = DateTime.Now;
            lp.sms = _sms;

            sup.trn_log_pos.InsertOnSubmit(lp);
            sup.SubmitChanges();
        }

        private void setPath()
        {
            
            string pas = Application.StartupPath;
            string sourcePath = pas;
            //string destinationPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup";
            string destinationPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string sourceFileName = "AutoPos.lnk";//eny tipe of file
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = System.IO.Path.Combine(destinationPath, sourceFileName);

            if (!System.IO.Directory.Exists(destinationFile))
            {
                System.IO.File.Copy(sourceFile, destinationFile, true);
            }
           
        }


        private void setWhcode()
        {
            int _wh_id = cmd.DEF_LOCALs.Select(s => s.WH_ID).FirstOrDefault();
            string _whcode = cmd.MAS_WHs.Where(s=>s.ID == _wh_id).Select(s=>s.WHCODE).FirstOrDefault();

            Whcode = _whcode;
        }


        #region update

        private void getText()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                //รันโปรแกรมจากการติดตั้งของ Clickonce
                //ดึงเวอร์ชั่นไปแสดงบนไตเติ้ล ของฟอร์ม และป้ายชื่อ(Label)
                string verion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                //this.Text += " - kBeautyCommSupport (" +  ":"  + ":" + _cBeautyComm.whname + "[" + _cBeautyComm.tmcode + "])";
                this.Text += " Version : " + verion;
                //lblVersion.Text = "Version : " + verion;
            }
            else
            {
                //รันจากการ Debug โปรแกรมบน VS2008
                //ให้แสดงข้อมความอื่น บนไตเติ้ล ของฟอร์ม และป้ายชื่อ
                //this.Text += " - kBeautyCommSupport (" + _cBeautyComm.wh_id + ":" + _cBeautyComm.whcode + ":" + _cBeautyComm.whname + "[" + _cBeautyComm.tmcode + "])";
                this.Text += " Version : [not deployed via ClickOnce]";
                //lblVersion.Text = "Version : [not deployed via ClickOnce]";
            }
        }

        private void UpdateApplication()
        {
            //ถ้าเป็นการรันโปรแกรมจากการติดตั้งด้วย ClickOnce 
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                //ประกาศตัวแปร checkUpdate เป็นประเภท ApplicationDeployment
                //กำหนดค่าให้มันเป็น CurrentDeployment ตัวที่ Deploy ล่าสุด
                ApplicationDeployment checkUpdate = ApplicationDeployment.CurrentDeployment;

                //สร้าง Event checkUpdate_CheckForUpdateCompleted
                checkUpdate.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(checkUpdate_CheckForUpdateCompleted);

                //สร้างEvent checkUpdate_CheckForUpdateProgressChanged
                checkUpdate.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(checkUpdate_CheckForUpdateProgressChanged);

                //เรียกเมธอด CheckForUpdateAsync:ตรวจสอบเวอร์ชั่นใหม่บนเซอร์ฟเวอร์
                checkUpdate.CheckForUpdateAsync();

                //เรียกเมธอด แสดง ProgrssBar
                showProgrssBar();
            }
        }

        void checkUpdate_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            //ประกาศตัวแปร progress เพื่อเก็บข้อมูลขนาดไฟล์ ของเวอร์ชั่นใหม่บนเซอร์ฟเวอร์
            string progress = String.Format("Downloading: {0}. {1:D}K of {2:D}K downloaded.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);


            //นำข้อมูลใน progress ไปแสดงบนป้ายชื่อของ toolStripStatusLabel1.Text, label1.Text  ของฟอร์ม frmProgress
            frmpro.toolStripStatusLabel1.Text = progress;
            frmpro.label1.Text = progress;


            //นำค่าเปอร์เซ็นต์ความคืบหน้างานแบบอะซิงโครนัสที่ได้รับ จากตัวแปร e ไปแสดงบน toolStripProgressBar1 ของฟอร์ม frmProgress
            frmpro.toolStripProgressBar1.Value = e.ProgressPercentage;


        }

        void checkUpdate_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            ////ปิดฟอร์ม frmProgress 
            frmpro.Close();

            if (e.Error != null)
            {
                MessageBox.Show("ERROR: Could not retrieve new version of the application. Reason: \n" +
                    e.Error.Message + "\nPlease report this error to the system administrator.");
                return;
            }
            else if (e.Cancelled == true)
            {
                MessageBox.Show("The update was cancelled.");
            }

            //ถ้าเป็นการ Update
            if (e.UpdateAvailable)
            {

                //ประกาศตัวแปร verion โดยดึงค่าเวอร์ชั่นใหม่ บนเซอร์เวอร์
                string verion = e.AvailableVersion.ToString();

                //โดย MessageBox แจ้งให้ผู้ใช้งานทราบ ว่ามีเวอร์ชั่นใหม่ 
                //ขณะเดียวกันก็แสดงข้อมูลในตัวแปร smsUpdate ซึ่งสมมุติว่า เป็นการดึงรายการปรับปรุง ของเวอร์ชั่นใหม่ บนเซอร์ฟเวอร์ เพื่อให้ผู้ใช้งานทราบ 
                //MessageBox.Show("โปรแกรมมีเวอร์ชั่นใหม่ [" + verion + "]"
                //       + Environment.NewLine + smsUpdate + Environment.NewLine +
                //       "กรุณา Update เพื่อให้คุณทันสมัย...", "ผลการตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //เรียกเมธอด BeginUpdate เพื่อเริ่ม Update เวอร์ชั่นใหม่
                BeginUpdate();
            }
        }

        public void BeginUpdate()
        {
            //ประกาศตัวแปร beginUpdate เป็นประเภท ApplicationDeployment และกำหนดค่าให้เป็น CurrentDeployment (กล่าวคือโปรแกรมที่ติดตั้งล่าสุด ของเครื่องของ ผู้ใช้งาน)
            ApplicationDeployment beginUpdate = ApplicationDeployment.CurrentDeployment;

            //สร้าง Event beginUpdate_UpdateCompleted
            beginUpdate.UpdateCompleted += new AsyncCompletedEventHandler(beginUpdate_UpdateCompleted);

            //สร้าง Event beginUpdate_UpdateProgressChanged
            beginUpdate.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(beginUpdate_UpdateProgressChanged);

            //เรียกเมธอด UpdateAsync เพื่อ Update แบบอะซิงโครนัส 
            beginUpdate.UpdateAsync();

            //เรียกเมธอด showProgrssBar
            //showProgrssBar();
        }

        void beginUpdate_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {

            //ประกาศตัวแปร progress เพื่อเก็บข้อมูลขนาดไฟล์ ของเวอร์ชั่นใหม่บนเซอร์ฟเวอร์
            String progressText = String.Format("{0:D}K out of {1:D}K downloaded - {2:D}% complete", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);

            //นำข้อมูลใน progress ไปแสดงบนป้ายชื่อของ toolStripStatusLabel1.Text, label1.Text  ของฟอร์ม frmProgress
            frmpro.toolStripStatusLabel1.Text = progressText;
            frmpro.label1.Text = progressText;

            //นำค่าเปอร์เซ็นต์ความคืบหน้างานแบบอะซิงโครนัสที่ได้รับ จากตัวแปร e ไปแสดงบน toolStripProgressBar1 ของฟอร์ม frmProgress
            frmpro.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        void beginUpdate_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ////ปิดฟอร์ม frmProgress 
            //frmpro.Close();

            if (e.Cancelled)
            {
                MessageBox.Show("The update of the application's latest version was cancelled.");
                return;
            }
            else if (e.Error != null)
            {
                MessageBox.Show("ERROR: Could not install the latest version of the application. Reason: \n" + e.Error.Message + "\nPlease report this error to the system administrator.");
                return;
            }

            //เมื่อ Update เรียบร้อยแล้ว ClickOnce จะ ปิดและเปิดโปรแกรม ให้ใหม่
            Application.Restart();
        }

        public string GetProgressString(DeploymentProgressState state)
        {

            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }
            else
            {
                return "deployment manifest";
            }
        }


        public void showProgrssBar()
        {
            ////สร้างออบเจ็กต์ ใหม่ให้ตัวแปร frmProgress
            //frmUpdateProgress = new frmUpdateProgress();


            //ลบค่าบนป้าย toolStripStatusLabel1
            frmpro.toolStripStatusLabel1.Text = string.Empty;

            ////ลบค่าบนป้าย toolStripProgressBar1
            frmpro.toolStripProgressBar1.Value = 0;

            //แสดงฟอร์ม frmProgress
            frmpro.ShowDialog();
            //frmpro.Close();
        }

        #endregion
    }



}

 

