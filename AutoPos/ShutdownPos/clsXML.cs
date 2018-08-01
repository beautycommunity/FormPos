using k.libary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShutdownPos
{
    class clsXML
    {
        public void setXML()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\kBeautyCommSupport\config.xml");

            XmlNodeList aNodes = doc.SelectNodes("/config/auto/timesale");

            aNodes.Item(0).InnerText = "0";

            doc.Save(@"C:\kBeautyCommSupport\config.xml");
        }

        public string getLocalStr()
        {

            string strlocal = "";

            string _localpos_servername = "";
            string _localpos_port = "";
            string _localpos_dbname = "";
            string _localpos_userid = "";
            string _localpos_userpass = "";

            string path = @"C:\kBeautyCommSupport\config.xml";
            if (!File.Exists(path))
            {
                cMessage.ErrorMessageNoData();
                return "";
            }

            DataSet ds = new DataSet();
            ds.ReadXml(path);

            foreach (DataRow dr in ds.Tables[2].Rows)
            {
                _localpos_servername = dr[0].ToString();
                _localpos_port = dr[1].ToString();
                _localpos_dbname = dr[2].ToString();
                _localpos_userid = dr[3].ToString();
                _localpos_userpass = dr[4].ToString();

            }

            if (_localpos_servername != "")
            {
                strlocal = "Data Source=" + _localpos_servername + "," + _localpos_port + "; Initial Catalog=" + _localpos_dbname + ";User ID=" + _localpos_userid + ";password=" + _localpos_userpass;
            }
            else
            {
                strlocal = "Data Source=(local)\\sqlexpress;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            }



            return strlocal;
        }

        public string getSupStr()
        {

            string strlocal = "";

            string _localpos_servername = "";
            string _localpos_port = "";
            string _localpos_dbname = "";
            string _localpos_userid = "";
            string _localpos_userpass = "";

            string path = @"C:\kBeautyCommSupport\config.xml";
            if (!File.Exists(path))
            {
                cMessage.ErrorMessageNoData();
                return "";
            }

            DataSet ds = new DataSet();
            ds.ReadXml(path);

            foreach (DataRow dr in ds.Tables[3].Rows)
            {
                _localpos_servername = dr[0].ToString();
                _localpos_port = dr[1].ToString();
                _localpos_dbname = dr[2].ToString();
                _localpos_userid = dr[3].ToString();
                _localpos_userpass = dr[4].ToString();

            }

            if (_localpos_servername != "")
            {
                strlocal = "Data Source=" + _localpos_servername + "," + _localpos_port + "; Initial Catalog=" + _localpos_dbname + ";User ID=" + _localpos_userid + ";password=" + _localpos_userpass;
            }
            else
            {
                strlocal = "Data Source=(local)\\sqlexpress;Initial Catalog=dbBeautycommsupport;User ID=sa;password=0000";
            }



            return strlocal;
        }
    }
}
