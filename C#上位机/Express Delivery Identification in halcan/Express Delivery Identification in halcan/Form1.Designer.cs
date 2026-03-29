namespace Express_Delivery_Identification_in_halcan
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.lblImageCount = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnRefreshPort = new System.Windows.Forms.Button();
            this.lblPortStatus = new System.Windows.Forms.Label();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtReceive = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(-11, -3);
            this.hWindowControl1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(906, 702);
            this.hWindowControl1.TabIndex = 0;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(906, 702);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(941, 23);
            this.btnSelectFolder.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(270, 40);
            this.btnSelectFolder.TabIndex = 1;
            this.btnSelectFolder.Text = "选择图片文件夹";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(941, 87);
            this.txtFolderPath.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(266, 31);
            this.txtFolderPath.TabIndex = 2;
            this.txtFolderPath.TextChanged += new System.EventHandler(this.txtFolderPath_TextChanged);
            // 
            // lblImageCount
            // 
            this.lblImageCount.Location = new System.Drawing.Point(1245, 101);
            this.lblImageCount.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblImageCount.Name = "lblImageCount";
            this.lblImageCount.Size = new System.Drawing.Size(117, 55);
            this.lblImageCount.TabIndex = 3;
            this.lblImageCount.Text = "label1";
            this.lblImageCount.Click += new System.EventHandler(this.lblImageCount_Click);
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(952, 301);
            this.cmbPort.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(218, 29);
            this.cmbPort.TabIndex = 4;
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Location = new System.Drawing.Point(952, 376);
            this.cmbBaud.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(218, 29);
            this.cmbBaud.TabIndex = 5;
            this.cmbBaud.Text = "选波特率";
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(1213, 301);
            this.btnOpenPort.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(239, 104);
            this.btnOpenPort.TabIndex = 6;
            this.btnOpenPort.Text = "打开串口";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 705);
            this.txtLog.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(884, 156);
            this.txtLog.TabIndex = 7;
            this.txtLog.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(941, 150);
            this.btnProcess.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(270, 100);
            this.btnProcess.TabIndex = 8;
            this.btnProcess.Text = "识别当前图片";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(1223, 150);
            this.btnNext.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(229, 100);
            this.btnNext.TabIndex = 9;
            this.btnNext.Text = "下一张图";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnRefreshPort
            // 
            this.btnRefreshPort.Location = new System.Drawing.Point(952, 431);
            this.btnRefreshPort.Name = "btnRefreshPort";
            this.btnRefreshPort.Size = new System.Drawing.Size(218, 80);
            this.btnRefreshPort.TabIndex = 11;
            this.btnRefreshPort.Text = "刷新串口";
            this.btnRefreshPort.UseVisualStyleBackColor = true;
            this.btnRefreshPort.Click += new System.EventHandler(this.btnRefreshPort_Click);
            // 
            // lblPortStatus
            // 
            this.lblPortStatus.Location = new System.Drawing.Point(1209, 448);
            this.lblPortStatus.Name = "lblPortStatus";
            this.lblPortStatus.Size = new System.Drawing.Size(129, 46);
            this.lblPortStatus.TabIndex = 12;
            this.lblPortStatus.Text = "未连接";
            this.lblPortStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSend
            // 
            this.txtSend.Location = new System.Drawing.Point(972, 650);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(331, 31);
            this.txtSend.TabIndex = 13;
            this.txtSend.TextChanged += new System.EventHandler(this.txtSend_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(1348, 633);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(104, 61);
            this.btnSend.TabIndex = 14;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtReceive
            // 
            this.txtReceive.Location = new System.Drawing.Point(972, 705);
            this.txtReceive.Multiline = true;
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.ReadOnly = true;
            this.txtReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceive.Size = new System.Drawing.Size(480, 156);
            this.txtReceive.TabIndex = 15;
            this.txtReceive.TextChanged += new System.EventHandler(this.txtReceive_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1546, 875);
            this.Controls.Add(this.txtReceive);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.lblPortStatus);
            this.Controls.Add(this.btnRefreshPort);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnOpenPort);
            this.Controls.Add(this.cmbBaud);
            this.Controls.Add(this.cmbPort);
            this.Controls.Add(this.lblImageCount);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.hWindowControl1);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Label lblImageCount;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnRefreshPort;
        private System.Windows.Forms.Label lblPortStatus;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtReceive;
    }
}

