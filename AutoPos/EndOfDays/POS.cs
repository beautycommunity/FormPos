using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndOfDays
{

    public class POS
    {
        public POSPTS POSPT { get; set; }
        public List<POSPIS> POSPI { get; set; }
        public List<POSPTPRS> POSPTPR { get; set; }
        public string CREATEBY { get; set; }
        public string ENDDAY { get; set; }
        public string BRAND { get; set; }
    }

    public class POSPTENDDAY
    {
        public int WH_ID { get; set; }
        public DateTime ENDDAY_DATE { get; set; }
        public string ENDDAY_BY { get; set; }
        public string ENDDAY { get; set; }
        public string BRAND { get; set; }
    }

    public class Result
    {
        //public int StatusCode { get; set; }
        //public string Messages { get; set; }
        //public int Records { get; set; }
        //public object Results { get; set; }

        public string StatusCode { get; set; }
        public string Messages { get; set; }
        public string Records { get; set; }
        public string Results { get; set; }
    }

    public class JSONSTRING
    {
        public string DATAJSON { get; set; }
        public POSPTENDDAY POSENDDAY { get; set; }
    }


    public class POSPTS
    {
        public int WH_ID { get; set; }
        public string TMCODE { get; set; }
        public string ABBNO { get; set; }
        public DateTime PTDATE { get; set; }
        public DateTime PTTIME { get; set; }
        public short SHIFTNO { get; set; }
        public string PTSTATUS { get; set; }
        public string OCSTATUS { get; set; }
        public DateTime WORKDATE { get; set; }
        public int ST_ID_LOG { get; set; }
        public int ST_ID { get; set; }
        public string IV_DOCNO { get; set; }
        public string CN_DOCNO { get; set; }
        public string REFNO { get; set; }
        public string CT_ID { get; set; }
        public string CT_BILL_TO { get; set; }
        public string CT_BILL_ADDR1 { get; set; }
        public string CT_BILL_ADDR2 { get; set; }
        public string CT_BILL_ADDR3 { get; set; }
        public string CT_CARDID { get; set; }
        public string CTTYPE { get; set; }
        public string CTCAT { get; set; }
        public int? CO_ID { get; set; }
        public string CO_CARDID { get; set; }
        public Decimal? POINT { get; set; }
        public Decimal? POINT_TOTAL { get; set; }
        public string SDPCODE { get; set; }
        public string PRCODE { get; set; }
        public string REMARK { get; set; }
        public string VATTYPE { get; set; }
        public Decimal VATRATE { get; set; }
        public Decimal QTY { get; set; }
        public Decimal SUMMARY { get; set; }
        public Decimal ITEMDISCOUNT { get; set; }
        public Decimal SUBTOTAL { get; set; }
        public Decimal SUBTOTAL_NORM { get; set; }
        public string DISCOUNTSTR { get; set; }
        public Decimal DISCOUNTAMT { get; set; }
        public Decimal DISCOUNTAMT_VAT { get; set; }
        public Decimal TOTAL { get; set; }
        public Decimal TOTAL_VAT { get; set; }
        public Decimal VATAMT { get; set; }
        public Decimal ROUNDAMT { get; set; }
        public Decimal NET { get; set; }
        public Decimal? VAL { get; set; }
        public Decimal? PAY_OTHER { get; set; }
        public Decimal? PAY_CASH { get; set; }
        public Decimal? PAY_CASH_TENDER { get; set; }
        public Decimal? PAY_CARD1 { get; set; }
        public int? PAY_CARD_CD_ID1 { get; set; }
        public string PAY_CARD_ID1 { get; set; }
        public string PAY_CARD_NAME1 { get; set; }
        public Decimal? PAY_CARD2 { get; set; }
        public int? PAY_CARD_CD_ID2 { get; set; }
        public string PAY_CARD_ID2 { get; set; }
        public string PAY_CARD_NAME2 { get; set; }
        public string RF_IV_DOCNO { get; set; }
        public Decimal? RF_IV_RMVALUE { get; set; }
        public Decimal? RF_RIGHTAMT { get; set; }
        public Decimal? RF_DIFAMT { get; set; }
        public Decimal? TAG_F { get; set; }
        public string TAG_S { get; set; }
    }

    public class POSPIS
    {
        public int WH_ID { get; set; }
        public string TMCODE { get; set; }
        public string ABBNO { get; set; }
        public DateTime PTDATE { get; set; }
        public short ITEMNO { get; set; }
        public int MP_ID { get; set; }
        public int BC_ID { get; set; }
        public string VATABLE { get; set; }
        public string CONTROLSN { get; set; }
        public int SH_ID { get; set; }
        public Decimal? ACPRICE { get; set; }
        public short PRICENO { get; set; }
        public string DPRICE { get; set; }
        public string BPRICE { get; set; }
        public Decimal PRICE { get; set; }
        public int UM_ID { get; set; }
        public Decimal UMRATIO { get; set; }
        public Decimal QTY { get; set; }
        public string DISCOUNTSTR { get; set; }
        public Decimal DISCOUNTAMT { get; set; }
        public Decimal TOTAL { get; set; }
        public Decimal NET { get; set; }
        public Decimal? VAL { get; set; }
        public Decimal? POINT { get; set; }
        public string PRCODE { get; set; }
        public int? OFFERID { get; set; }
        public string ITEMAU { get; set; }
        public int? ITEMRF { get; set; }
        public Decimal? QTY_RF { get; set; }
        public Decimal? PR_TOTAL { get; set; }
        public Decimal? PR_NET { get; set; }
        public Decimal? PR_VAL { get; set; }
        public string PIVOIDED { get; set; }
        public string PISTATUS { get; set; }
        public DateTime WORKDATE { get; set; }
        public Decimal? TAG_F { get; set; }
        public string TAG_S { get; set; }
    }

    public class POSPTPRS
    {
        public int WH_ID { get; set; }
        public string TMCODE { get; set; }
        public string ABBNO { get; set; }
        public DateTime PTDATE { get; set; }
        public string PRCODE { get; set; }
        public int ITEMNO { get; set; }
        public int? SORTNO { get; set; }
        public string CVC_NO { get; set; }
        public Decimal? DVAL { get; set; }
        public string DTYPE { get; set; }
        public Decimal? TOTALB4DISC { get; set; }
        public Decimal? DISCOUNTAMT { get; set; }
        public DateTime? WORKDATE { get; set; }
    }
}
