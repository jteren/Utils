namespace SQLEventProfiler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cbxServer = new ComboBox();
            btnStart = new Button();
            btnStop = new Button();
            lblServer = new Label();
            stsStatus = new StatusStrip();
            stsStatusLabel = new ToolStripStatusLabel();
            groupBox1 = new GroupBox();
            txtUserToLog = new TextBox();
            cbxThisMachine = new CheckBox();
            grpAuthentication = new GroupBox();
            btnPasswordSwapper = new Button();
            txtPassword = new TextBox();
            txtUserName = new TextBox();
            cbxAuthenticationType = new ComboBox();
            ofd = new OpenFileDialog();
            txtLogFile = new TextBox();
            lblLogFile = new Label();
            btnSelectFile = new Button();
            lblStopwatch = new Label();
            stsStatus.SuspendLayout();
            groupBox1.SuspendLayout();
            grpAuthentication.SuspendLayout();
            SuspendLayout();
            // 
            // cbxServer
            // 
            cbxServer.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxServer.FormattingEnabled = true;
            cbxServer.Location = new Point(31, 55);
            cbxServer.Name = "cbxServer";
            cbxServer.Size = new Size(326, 38);
            cbxServer.TabIndex = 0;
            cbxServer.SelectedIndexChanged += cbxServer_SelectedIndexChanged;
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Exo 2", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(418, 25);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(434, 129);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Exo 2", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(418, 160);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(434, 129);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Font = new Font("Segoe UI", 11F);
            lblServer.Location = new Point(28, 25);
            lblServer.Name = "lblServer";
            lblServer.Size = new Size(76, 30);
            lblServer.TabIndex = 5;
            lblServer.Text = "Server";
            // 
            // stsStatus
            // 
            stsStatus.ImageScalingSize = new Size(24, 24);
            stsStatus.Items.AddRange(new ToolStripItem[] { stsStatusLabel });
            stsStatus.Location = new Point(0, 482);
            stsStatus.Name = "stsStatus";
            stsStatus.Size = new Size(864, 22);
            stsStatus.TabIndex = 6;
            stsStatus.Text = "statusStrip1";
            // 
            // stsStatusLabel
            // 
            stsStatusLabel.Name = "stsStatusLabel";
            stsStatusLabel.Size = new Size(0, 15);
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtUserToLog);
            groupBox1.Controls.Add(cbxThisMachine);
            groupBox1.Location = new Point(31, 112);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(326, 132);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log for:";
            // 
            // txtUserToLog
            // 
            txtUserToLog.Location = new Point(17, 79);
            txtUserToLog.Name = "txtUserToLog";
            txtUserToLog.PlaceholderText = "Username to log for";
            txtUserToLog.Size = new Size(278, 37);
            txtUserToLog.TabIndex = 1;
            // 
            // cbxThisMachine
            // 
            cbxThisMachine.AutoSize = true;
            cbxThisMachine.Location = new Point(17, 39);
            cbxThisMachine.Name = "cbxThisMachine";
            cbxThisMachine.Size = new Size(109, 34);
            cbxThisMachine.TabIndex = 0;
            cbxThisMachine.Text = "This PC";
            cbxThisMachine.UseVisualStyleBackColor = true;
            cbxThisMachine.CheckStateChanged += cbxThisMachine_CheckStateChanged;
            // 
            // grpAuthentication
            // 
            grpAuthentication.Controls.Add(btnPasswordSwapper);
            grpAuthentication.Controls.Add(txtPassword);
            grpAuthentication.Controls.Add(txtUserName);
            grpAuthentication.Controls.Add(cbxAuthenticationType);
            grpAuthentication.Location = new Point(28, 265);
            grpAuthentication.Name = "grpAuthentication";
            grpAuthentication.Size = new Size(329, 186);
            grpAuthentication.TabIndex = 11;
            grpAuthentication.TabStop = false;
            grpAuthentication.Text = "Authentication";
            // 
            // btnPasswordSwapper
            // 
            btnPasswordSwapper.BackgroundImage = Properties.Resources.arrows;
            btnPasswordSwapper.BackgroundImageLayout = ImageLayout.Zoom;
            btnPasswordSwapper.Location = new Point(233, 129);
            btnPasswordSwapper.Name = "btnPasswordSwapper";
            btnPasswordSwapper.Size = new Size(65, 38);
            btnPasswordSwapper.TabIndex = 3;
            btnPasswordSwapper.UseVisualStyleBackColor = true;
            btnPasswordSwapper.Click += btnPasswordSwapper_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(20, 129);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(278, 37);
            txtPassword.TabIndex = 2;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(20, 86);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(278, 37);
            txtUserName.TabIndex = 1;
            // 
            // cbxAuthenticationType
            // 
            cbxAuthenticationType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxAuthenticationType.FormattingEnabled = true;
            cbxAuthenticationType.Location = new Point(20, 36);
            cbxAuthenticationType.Name = "cbxAuthenticationType";
            cbxAuthenticationType.Size = new Size(278, 38);
            cbxAuthenticationType.TabIndex = 0;
            cbxAuthenticationType.SelectedIndexChanged += cbxAuthenticationType_SelectedIndexChanged;
            // 
            // txtLogFile
            // 
            txtLogFile.Enabled = false;
            txtLogFile.Location = new Point(418, 394);
            txtLogFile.Name = "txtLogFile";
            txtLogFile.PlaceholderText = "Select a file...";
            txtLogFile.Size = new Size(431, 37);
            txtLogFile.TabIndex = 12;
            // 
            // lblLogFile
            // 
            lblLogFile.AutoSize = true;
            lblLogFile.Font = new Font("Segoe UI", 11F);
            lblLogFile.Location = new Point(418, 361);
            lblLogFile.Name = "lblLogFile";
            lblLogFile.Size = new Size(84, 30);
            lblLogFile.TabIndex = 13;
            lblLogFile.Text = "Log file";
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new Point(803, 392);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(46, 39);
            btnSelectFile.TabIndex = 15;
            btnSelectFile.Text = "+";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // lblStopwatch
            // 
            lblStopwatch.AutoSize = true;
            lblStopwatch.BackColor = Color.White;
            lblStopwatch.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStopwatch.ForeColor = Color.DarkGray;
            lblStopwatch.Location = new Point(568, 116);
            lblStopwatch.Name = "lblStopwatch";
            lblStopwatch.Size = new Size(133, 32);
            lblStopwatch.TabIndex = 16;
            lblStopwatch.Text = "00:00:00:00";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(864, 504);
            Controls.Add(lblStopwatch);
            Controls.Add(btnSelectFile);
            Controls.Add(lblLogFile);
            Controls.Add(txtLogFile);
            Controls.Add(grpAuthentication);
            Controls.Add(groupBox1);
            Controls.Add(stsStatus);
            Controls.Add(lblServer);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(cbxServer);
            Name = "Form1";
            Text = "XE Profiler";
            FormClosing += Form1_FormClosing;
            stsStatus.ResumeLayout(false);
            stsStatus.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpAuthentication.ResumeLayout(false);
            grpAuthentication.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbxServer;
        private Button btnStart;
        private Button btnStop;
        private Label lblServer;
        private StatusStrip stsStatus;
        private GroupBox groupBox1;
        private TextBox txtUserToLog;
        private CheckBox cbxThisMachine;
        private GroupBox grpAuthentication;
        private TextBox txtUserName;
        private ComboBox cbxAuthenticationType;
        private TextBox txtPassword;
        private ToolStripStatusLabel stsStatusLabel;
        private OpenFileDialog ofd;
        private TextBox txtLogFile;
        private Label lblLogFile;
        private Button btnSelectFile;
        private Button btnPasswordSwapper;
        private Label lblStopwatch;
    }
}
