using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using k.libary;

namespace ShutdownPos
{
    public partial class frmMain : frmSub
    {
        frmUpdateProgress frmpro = new frmUpdateProgress();
        string smsUpdate = "";

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                smsUpdate = "ปรับปรุง:" + Environment.NewLine;

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

        public frmMain()
        {
            InitializeComponent();

            getText();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("คุณต้องการรีสตาร์ทเครื่องคอมใช่หรือไม่ ? ", "Restart", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start("shutdown", "/r /f /t 0");
            }
    
        }

        private void btnShutdown_Click(object sender, EventArgs e)
        {
            frmShutdown frm = new frmShutdown();
            frm.ShowDialog();
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

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                MessageBox.Show(ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
            }
            catch
            {
                MessageBox.Show("TEST");
            }
        }
    }
}
