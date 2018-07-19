using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoPos
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
    }
}
