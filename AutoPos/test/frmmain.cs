using EndOfDays;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using k.libary;

namespace test
{
    public partial class frmmain : frmMain
    {
        public frmmain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string StrConn = "Data Source=LTHY.DYNDNS.info,1401;Initial Catalog=CMD-FX;User ID=sa;password=0000";
            string StrConnSup = "Data Source=LTHY.DYNDNS.info,1401;Initial Catalog=dbBeautycommsupport;User ID=sa;password=0000";
            string Whcode = "1226";
            string Stcode = "2558";

           frmEOD frm = new frmEOD(StrConn, StrConnSup, Whcode, Stcode);
            frm.ShowDialog();
            
        }
    }
}
