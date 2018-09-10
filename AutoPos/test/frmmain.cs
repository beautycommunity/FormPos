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
           
            string StrConn = "Data Source=.;Initial Catalog=CMD-FX;User ID=sa;password=1Q2w3e4r@";
            string StrConnSup = "Data Source=.;Initial Catalog=dbBeautyCommSupport;User ID=sa;password=1Q2w3e4r@";
            string Whcode = "1226";
            string Stcode = "2558";

           frmEOD frm = new frmEOD(StrConn, StrConnSup, Whcode, Stcode);
            frm.ShowDialog();
            
        }
    }
}
