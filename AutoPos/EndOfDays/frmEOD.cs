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
using RestSharp;
using RestSharp.Deserializers;

using System.Data.SqlClient;

namespace EndOfDays
{
    public partial class frmEOD : frmSubWithMenuButton
    {
        string StrConn;
        string StrConnSup;
        string Whcode;
        string Stcode;
        string sms;
        CultureInfo us = CultureInfo.GetCultureInfo("en-US");

        //List<POS> ListPOS;
        //List<POSPIS> ListPI;
        //List<POSPTPRS> ListPTPR;
        List<POS> ListPOS = new List<POS>();

        CMDDataContext cmd = new CMDDataContext();
        POSULDataContext sup = new POSULDataContext();

        public frmEOD()
        {
            InitializeComponent();

            //StrConn = "Data Source=(local)/sqlexpress;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            //StrConnSup = "Data Source=(local)/sqlexpress;Initial Catalog=dbBeautyCommSupport;User ID=sa;password=0000";
            StrConn = "Data Source=CENL.DYNDNS.info,1401;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            StrConnSup = "Data Source=CENL.DYNDNS.info,1401;Initial Catalog=dbBeautycommsupport;User ID=sa;password=0000";
            Whcode = "1001";
            Stcode = "2558";

            //StrConn = "Data Source=.;Initial Catalog=CMD-FX;User ID=sa;password=1Q2w3e4r@";
            //StrConnSup = "Data Source=.;Initial Catalog=dbBeautyCommSupport;User ID=sa;password=1Q2w3e4r@";
            //Whcode = "1006";
            //Stcode = "2558";
        }

        public frmEOD(string _strconn, string _strconnsup, string _whcode,string _stcode)
        {
            InitializeComponent();
            StrConn = _strconn;
            StrConnSup = _strconnsup;
            Whcode = _whcode;
            Stcode = _stcode;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    
        private void frmEOD_Load(object sender, EventArgs e)
        {
            bgw.WorkerReportsProgress = true;
            CheckForIllegalCrossThreadCalls = false;

            cmd.Connection.ConnectionString = StrConn;
            sup.Connection.ConnectionString = StrConnSup;

            cmd.CommandTimeout = 1000000;
            sup.CommandTimeout = 1000000;

           try
            {
               if(editPV())
                {

                }

                loadData();

            }
            catch(Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด\n" + ex.Message+"pro4");
            }
        }

        private bool loadData()
        {
            bool bl = false;
            DateTime dt = DateTime.Now;

            if (insPosUL() == true)
            {
                var Rs = cmd.PF_GET_SALES().FirstOrDefault();

                if (Rs == null)
                {
                    decimal ul = sup.POS_ULs.Where(s => s.UFLAG == "Y" && s.WORKDATE == DateTime.Now.Date).Count();

                    lv.Items.Clear();
                    lblNone.Text = "-";
                    lblSend.Text = ul.ToString("#,##0");
                }
                else
                {
                    decimal decSend = Convert.ToDecimal(Rs.Ycnt);
                    decimal decNone = Convert.ToDecimal(Rs.Ncnt);

                    lblNone.Text = decNone.ToString("#,##0") == 0.ToString() ? "-" : decNone.ToString("#,##0");
                    lblSend.Text = decSend.ToString("#,##0") == 0.ToString() ? "-" : decSend.ToString("#,##0");


                }

                var data = cmd.PV_None_Syncs.ToList();

                lv.Items.Clear();
                ListViewItem LvItm = new ListViewItem();

                foreach (var item in data)
                {
                    LvItm = lv.Items.Add(item.rw.ToString());
                    int idx = lv.Items.IndexOf(LvItm);
                    lv.Items[idx].SubItems.Add(item.whcode);
                    lv.Items[idx].SubItems.Add(item.ABBNAME);
                    lv.Items[idx].SubItems.Add(item.ABBNO);
                    lv.Items[idx].SubItems.Add(item.TMCODE);
                    lv.Items[idx].SubItems.Add(item.WORKDATE);
                    lv.Items[idx].SubItems.Add(item.ptstatus);
                    lv.Items[idx].SubItems.Add(Convert.ToDecimal(item.NET).ToString("#,##0.00"));
                }

                bl = true;
            }
           

            return bl;
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                //ListPOS = new List<POS>();
                //ListPI = new List<POSPIS>();
                //ListPTPR = new List<POSPTPRS>();
                ListPOS.Clear();

                if (lv.Items.Count > 0)
                {
                    //autoSend();
                    //loadData();

                    pb.Location = new Point((this.Width - pb.Width) / 2, (this.Height - pb.Height) / 2);
                    pb.Visible = true;
                    bgw.RunWorkerAsync();

                    //autoSend();
                    //loadData();
                }
                else
                {

                    noBillSend();

                    MessageBox.Show("ไม่มีบิลคงค้าง");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด\n" + ex.Message+"pro5");
            }
        }

        private void noBillSend()
        {
            var restClient = new RestClient("http://5cosmeda.homeunix.com:89/ApiFromPOS/api/POS/InsertBill");
            //var restClient = new RestClient("http://192.168.10.202/ApiFromPOS/api/POS/InsertBill");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;

           int _wh_id = cmd.DEF_LOCALs.Select(s => s.WH_ID).FirstOrDefault();

            string sbrand = "";
            string fstr;
            fstr = Whcode.Substring(0, 1);

            if (fstr == "1" || fstr == "3")
            {
                sbrand = "BB";
            }
            else if (fstr == "5")
            {
                sbrand = "BC";
            }
            else
            {
                sbrand = "BM";
            }

            POSPTENDDAY PEnd = new POSPTENDDAY{WH_ID= _wh_id, ENDDAY="Y", ENDDAY_BY=Stcode,BRAND= sbrand };

            
            var json = JsonConvert.SerializeObject(PEnd) ;

            JSONSTRING ss = new JSONSTRING();
            ss.DATAJSON = "";
            ss.POSENDDAY = PEnd;

            //request.AddJsonBody(ss);

            request.AddBody(ss);

            var response = restClient.Execute(request);

            JsonDeserializer deserial = new JsonDeserializer();

            if ((int)response.StatusCode == 200)
            {
                List<Result> bl = deserial.Deserialize<List<Result>>(response);


                var item = bl.FirstOrDefault();

                if (item.StatusCode == "1")
                {

                    sms = "สำเร็จ";

                }
                else
                {
                    sms = item.Messages+"debug1";

                }
            }
            else
            {
                sms = response.ErrorException.Message + "Pro1";
                
            }
        }

        private bool autoSend() 
        {
            bool abl = false;

            if (getABBNO())
            {
                
                var restClient = new RestClient("http://5cosmeda.homeunix.com:89/ApiFromPOS/api/POS/InsertBill");
                //var restClient = new RestClient("http://192.168.10.202/ApiFromPOS/api/POS/InsertBill");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;


                var json = JsonConvert.SerializeObject(ListPOS);

                JSONSTRING ss = new JSONSTRING();
                ss.DATAJSON = json;

                //request.AddJsonBody(ss);
                request.AddBody(ss);

               var response = restClient.Execute(request);

                JsonDeserializer deserial = new JsonDeserializer();

                if ((int)response.StatusCode == 200)
                {
                    List<Result> bl = deserial.Deserialize<List<Result>>(response);

                    var item = bl.FirstOrDefault();

                    if (item.StatusCode == "1")
                    {
                        upPosUL();
                        sms = "สำเร็จ";
                        abl = true;
                        //MessageBox.Show("สำเร็จ");
                    }
                    else if(item.StatusCode == "2")
                    {
                        
                        if (item.Messages.Substring(0, 9) == "Violation" || item.Messages == "Object reference not set to an instance of an object.")
                        {
                            sms = item.Messages + "debug2";
                            abl = false;
                        }
                        else
                        {
                            upPosUL();
                            sms = "สำเร็จ";
                            abl = true;
                        }
                    }
                    else
                    {
                        sms = item.Messages + "debug3";
                        abl = false;
                        //MessageBox.Show(item.Messages);
                    }
                }
                else
                {
                    sms = response.ErrorException.Message+"pro2";
                    abl = false;
                    //MessageBox.Show(response.StatusCode.ToString());
                }

            }

            return abl;

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


                //var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N" &&( s.ABBNO == "10789" ) ).ToList();
                //var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N" ).Take(1).ToList();
                var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N").ToList();

                foreach (var item in bill_rs)
                {
                    getList(item.WH_ID, item.TMCODE, item.WORKDATE, item.ABBNO);
                }


                bl = true;
            }
            catch (Exception ex)
            {
                sms = ex.Message+"TEST2";
                bl = false;
            }

            return bl;
        }

        private void getList(int _wh_id, string _tmcode, DateTime _workdate, string _abbno)
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
                TAG_S = sql_pt.TAG_S
            };

            foreach (var item in sql_pi)
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

            string sbrand = "";
            string fstr;
            fstr = Whcode.Substring(0, 1);

            if (fstr == "1" || fstr == "3")
            {
                sbrand = "BB";
            }
            else if (fstr == "5")
            {
                sbrand = "BC";
            }
            else
            {
                sbrand = "BM";
            }

            POS pos = new POS { POSPT = pt, POSPI = ListPI, POSPTPR = ListPTPR, CREATEBY = Stcode,ENDDAY = "Y" ,BRAND = sbrand};
            ListPOS.Add(pos);
           

        }

        private bool insPosUL()
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
                bl = true;
            }
            catch (Exception ex)
            {
                bl = false;
                MessageBox.Show(ex.Message+"pro3");
            }
            return bl;
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

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if ( autoSend())
            {
                if (loadData()== false)
                {
                    sms = "ไม่สามารถ load ข้อมูลได้" ;
                }
            }
            else
            {
                sms = "ไม่สามารถ รับ-ส่ง ข้อมูลได้ Test1";
            }
            
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pb.Visible = false;
            ShowSMS();
        }

        private void ShowSMS()
        {
            if(sms!="")
            {
                MessageBox.Show(sms);
            }
            
        }

        private bool editPV()
        {

            bool bl = false;
            SqlConnection conn = new SqlConnection(StrConn);
            SqlCommand comm = new SqlCommand();

            try
            {
                
                string sql = @"ALTER VIEW [dbo].[PV_None_Sync]
                            AS
                            select ROW_NUMBER() over(order by a.abbno) rw ,c.whcode,c.ABBNAME,a.ABBNO,a.TMCODE,convert(varchar(10),a.WORKDATE,103) as WORKDATE 
                            ,b.NET ,
                            case when b.ptstatus = 'S' then 'บิลขาย'
                                 when b.ptstatus = 'R' then 'บิลรับคืน'
	                             when b.ptstatus = 'V' then 'ยกเลิก' end as ptstatus
                            from (select wh_id,abbno,ptdate,workdate,tmcode,uflag from [dbo].[PV_POS_UL]
                                  union all
                                  select wh_id,abbno,ptdate,workdate,tmcode,uflag from [dbBeautyCommsupport]..pos_ul where UFLAG = 'N' ) as a
                            left join (select wh_id,abbno,ptdate,workdate,tmcode,net,PTSTATUS from pos_pt) b ON a.WH_ID = b.WH_ID AND a.ABBNO = b.ABBNO AND a.PTDATE = b.PTDATE AND a.TMCODE = b.TMCODE
                            left join (select id,whcode,abbname from mas_wh where id = (select wh_id from DEF_LOCAL)) c on a.WH_ID = c.id
                            ";

                comm.CommandText = sql;
                comm.CommandTimeout = 100000;
                comm.Connection = conn;

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                comm.ExecuteNonQuery();

                bl = true;
            }
            catch
            {
                bl = false;
              
            }
            finally
            {
                conn.Close();
            }

            return bl;
        }
    }
}
