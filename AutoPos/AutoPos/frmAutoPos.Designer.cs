namespace AutoPos
{
    partial class frmAutoPos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOk = new System.Windows.Forms.Button();
            this.ntf = new System.Windows.Forms.NotifyIcon(this.components);
            this.tm = new System.Windows.Forms.Timer(this.components);
            this.btn_test = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(35, 91);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 31);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "ปิด";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ntf
            // 
            this.ntf.Text = "notifyIcon1";
            this.ntf.Visible = true;
            this.ntf.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ntf_MouseDoubleClick);
            // 
            // tm
            // 
            this.tm.Interval = 300000;
            this.tm.Tick += new System.EventHandler(this.tm_Tick);
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(35, 35);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(75, 31);
            this.btn_test.TabIndex = 1;
            this.btn_test.Text = "ทดสอบ";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 166);
            this.ControlBox = false;
            this.Controls.Add(this.btn_test);
            this.Controls.Add(this.btnOk);
            this.Name = "frmMain";
            this.Text = "Auto POS";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.NotifyIcon ntf;
        private System.Windows.Forms.Timer tm;
        private System.Windows.Forms.Button btn_test;
    }
}

