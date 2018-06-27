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

namespace EndOfDays
{
    public partial class frmEOD : frmSubWithMenuButton
    {
        string StrConn;
        string StrConnSup;
        string Whcode;
        string Stcode;
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
            //Whcode = "1226";
            //Stcode = "2558";

            StrConn = "Data Source=.;Initial Catalog=CMD-FX;User ID=sa;password=1Q2w3e4r@";
            StrConnSup = "Data Source=.;Initial Catalog=dbBeautyCommSupport;User ID=sa;password=1Q2w3e4r@";
            Whcode = "1006";
            Stcode = "2558";
        }

        public frmEOD(string _strconn, string _whcode,string _stcode)
        {
            InitializeComponent();
            StrConn = _strconn;
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
            

            cmd.Connection.ConnectionString = StrConn;
            sup.Connection.ConnectionString = StrConnSup;

           try
            {
                loadData();

            }
            catch(Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด\n" + ex.Message);
            }
        }

        private void loadData()
        {
            
            DateTime dt = DateTime.Now;

            if (insPosUL() == true)
            {
                var Rs = cmd.PF_GET_SALES().FirstOrDefault();

                if (Rs == null)
                {
                    lv.Items.Clear();
                    lblNone.Text = "-";
                    lblSend.Text = "-";
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
            }
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                //ListPOS = new List<POS>();
                //ListPI = new List<POSPIS>();
                //ListPTPR = new List<POSPTPRS>();

                if (lv.Items.Count > 0)
                {
                    autoSend();
                    loadData();
                }
                else
                {
                    MessageBox.Show("ไม่มีบิลคงค้าง");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด\n" + ex.Message);
            }
        }

        private void autoSend()
        {
            if (getABBNO() == true)
            {
                //using (var client = new HttpClient())
                //{

                //    List<Result> model = new List<Result>();

                //    //client.BaseAddress = new Uri("http://5cosmeda.homeunix.com:89/ApiFromPOS/");
                //    client.BaseAddress = new Uri("http://192.168.10.202:89/ApiFromPOS/");
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //    var json = JsonConvert.SerializeObject(ListPOS);

                //    JSONSTRING ss = new JSONSTRING();
                //    ss.DATAJSON = json;

                //    var response = client.PostAsJsonAsync("api/POS/InsertBill", ss).Result;
                //    var jsonString = response.Content.ReadAsFormDataAsync();
                //    jsonString.Wait();
                //    model = JsonConvert.DeserializeObject<List<Result>>(jsonString.Result);

                //    if ((int)response.StatusCode == 200)
                //        {
                //            upPosUL();
                //            MessageBox.Show("สำเร็จ");
                //        }
                //        else
                //        {
                //            MessageBox.Show(response.StatusCode.ToString());
                //        }


                //}

                var restClient = new RestClient("http://5cosmeda.homeunix.com:89/ApiFromPOS/api/POS/InsertBill");
                //var restClient = new RestClient("http://192.168.10.202/ApiFromPOS/api/POS/InsertBill");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(ListPOS);

                JSONSTRING ss = new JSONSTRING();
                ss.DATAJSON = json;
                request.AddJsonBody(ss);
                var response = restClient.Execute(request);

                JsonDeserializer deserial = new JsonDeserializer();
                List<Result> bl = deserial.Deserialize<List<Result>>(response);

                var item = bl.FirstOrDefault();

                if (item.StatusCode == 1)
                {
                    upPosUL();
                    MessageBox.Show("สำเร็จ");
                }
                else
                {
                    MessageBox.Show(item.Messages);
                }
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


                //var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N" &&( s.ABBNO == "10789" ) ).ToList();
                //var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N" ).Take(2).ToList();
                var bill_rs = sup.POS_ULs.Where(s => s.UFLAG == "N").ToList();

                foreach (var item in bill_rs)
                {
                    getList(item.WH_ID, item.TMCODE, item.WORKDATE, item.ABBNO);
                }


                bl = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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

       
    }
}
