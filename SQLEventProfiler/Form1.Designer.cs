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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            cbxServer = new ComboBox();
            btnStart = new Button();
            btnStop = new Button();
            lblServer = new Label();
            stsStatus = new StatusStrip();
            stsStatusLabel = new ToolStripStatusLabel();
            groupBox1 = new GroupBox();
            txtUserToLog = new TextBox();
            cbxThisMachine = new CheckBox();
            chkClearLogBeforeStart = new CheckBox();
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
            mspMenu = new MenuStrip();
            btnShowFilterEditor = new Button();
            chkAlwaysOnTop = new CheckBox();
            stsStatus.SuspendLayout();
            groupBox1.SuspendLayout();
            grpAuthentication.SuspendLayout();
            SuspendLayout();
            // 
            // cbxServer
            // 
            cbxServer.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxServer.Font = new Font("Exo 2", 11.1428576F);
            cbxServer.FormattingEnabled = true;
            cbxServer.Location = new Point(31, 77);
            cbxServer.Name = "cbxServer";
            cbxServer.Size = new Size(369, 40);
            cbxServer.TabIndex = 0;
            cbxServer.SelectedIndexChanged += cbxServer_SelectedIndexChanged;
            // 
            // btnStart
            // 
            btnStart.Cursor = Cursors.Hand;
            btnStart.Font = new Font("Exo 2", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(418, 47);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(434, 129);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Cursor = Cursors.Hand;
            btnStop.Font = new Font("Exo 2", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(418, 182);
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
            lblServer.Font = new Font("Exo 2", 11F);
            lblServer.Location = new Point(28, 45);
            lblServer.Name = "lblServer";
            lblServer.Size = new Size(80, 32);
            lblServer.TabIndex = 5;
            lblServer.Text = "Server";
            // 
            // stsStatus
            // 
            stsStatus.AutoSize = false;
            stsStatus.Font = new Font("Exo 2", 11.1428576F);
            stsStatus.ImageScalingSize = new Size(24, 24);
            stsStatus.Items.AddRange(new ToolStripItem[] { stsStatusLabel });
            stsStatus.Location = new Point(0, 501);
            stsStatus.Name = "stsStatus";
            stsStatus.Size = new Size(876, 38);
            stsStatus.TabIndex = 6;
            stsStatus.Text = "statusStrip1";
            // 
            // stsStatusLabel
            // 
            stsStatusLabel.Name = "stsStatusLabel";
            stsStatusLabel.Size = new Size(0, 31);
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtUserToLog);
            groupBox1.Controls.Add(cbxThisMachine);
            groupBox1.Font = new Font("Exo 2", 11.1428576F);
            groupBox1.Location = new Point(31, 134);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(326, 132);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log";
            // 
            // txtUserToLog
            // 
            txtUserToLog.Location = new Point(17, 79);
            txtUserToLog.Name = "txtUserToLog";
            txtUserToLog.PlaceholderText = "PC/username";
            txtUserToLog.Size = new Size(278, 34);
            txtUserToLog.TabIndex = 1;
            // 
            // cbxThisMachine
            // 
            cbxThisMachine.AutoSize = true;
            cbxThisMachine.Location = new Point(17, 39);
            cbxThisMachine.Name = "cbxThisMachine";
            cbxThisMachine.Size = new Size(117, 38);
            cbxThisMachine.TabIndex = 0;
            cbxThisMachine.Text = "This PC";
            cbxThisMachine.UseVisualStyleBackColor = true;
            cbxThisMachine.CheckStateChanged += cbxThisMachine_CheckStateChanged;
            // 
            // chkClearLogBeforeStart
            // 
            chkClearLogBeforeStart.AutoSize = true;
            chkClearLogBeforeStart.Font = new Font("Exo 2", 11.1428576F);
            chkClearLogBeforeStart.Location = new Point(418, 418);
            chkClearLogBeforeStart.Margin = new Padding(3, 2, 3, 2);
            chkClearLogBeforeStart.Name = "chkClearLogBeforeStart";
            chkClearLogBeforeStart.Size = new Size(183, 38);
            chkClearLogBeforeStart.TabIndex = 18;
            chkClearLogBeforeStart.Text = "Clear on start";
            chkClearLogBeforeStart.UseVisualStyleBackColor = true;
            // 
            // grpAuthentication
            // 
            grpAuthentication.Controls.Add(btnPasswordSwapper);
            grpAuthentication.Controls.Add(txtPassword);
            grpAuthentication.Controls.Add(txtUserName);
            grpAuthentication.Controls.Add(cbxAuthenticationType);
            grpAuthentication.Font = new Font("Exo 2", 11.1428576F);
            grpAuthentication.Location = new Point(28, 287);
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
            btnPasswordSwapper.Cursor = Cursors.Hand;
            btnPasswordSwapper.Location = new Point(244, 128);
            btnPasswordSwapper.Name = "btnPasswordSwapper";
            btnPasswordSwapper.Size = new Size(54, 34);
            btnPasswordSwapper.TabIndex = 3;
            btnPasswordSwapper.UseVisualStyleBackColor = true;
            btnPasswordSwapper.Click += btnPasswordSwapper_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(20, 129);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(278, 34);
            txtPassword.TabIndex = 2;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(20, 86);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(278, 34);
            txtUserName.TabIndex = 1;
            // 
            // cbxAuthenticationType
            // 
            cbxAuthenticationType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxAuthenticationType.FormattingEnabled = true;
            cbxAuthenticationType.Location = new Point(20, 36);
            cbxAuthenticationType.Name = "cbxAuthenticationType";
            cbxAuthenticationType.Size = new Size(278, 40);
            cbxAuthenticationType.TabIndex = 0;
            // 
            // txtLogFile
            // 
            txtLogFile.Enabled = false;
            txtLogFile.Font = new Font("Exo 2", 11.1428576F);
            txtLogFile.Location = new Point(418, 372);
            txtLogFile.Name = "txtLogFile";
            txtLogFile.PlaceholderText = "Select a file...";
            txtLogFile.Size = new Size(431, 34);
            txtLogFile.TabIndex = 12;
            // 
            // lblLogFile
            // 
            lblLogFile.AutoSize = true;
            lblLogFile.Font = new Font("Exo 2", 11F);
            lblLogFile.Location = new Point(418, 340);
            lblLogFile.Name = "lblLogFile";
            lblLogFile.Size = new Size(87, 32);
            lblLogFile.TabIndex = 13;
            lblLogFile.Text = "Log file";
            // 
            // btnSelectFile
            // 
            btnSelectFile.BackgroundImage = (Image)resources.GetObject("btnSelectFile.BackgroundImage");
            btnSelectFile.BackgroundImageLayout = ImageLayout.Center;
            btnSelectFile.Cursor = Cursors.Hand;
            btnSelectFile.Location = new Point(803, 371);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(46, 34);
            btnSelectFile.TabIndex = 15;
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // lblStopwatch
            // 
            lblStopwatch.AutoSize = true;
            lblStopwatch.BackColor = Color.White;
            lblStopwatch.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStopwatch.ForeColor = Color.DarkGray;
            lblStopwatch.Location = new Point(568, 138);
            lblStopwatch.Name = "lblStopwatch";
            lblStopwatch.Size = new Size(133, 32);
            lblStopwatch.TabIndex = 16;
            lblStopwatch.Text = "00:00:00:00";
            // 
            // mspMenu
            // 
            mspMenu.Font = new Font("Exo 2", 11.1428576F);
            mspMenu.ImageScalingSize = new Size(24, 24);
            mspMenu.Location = new Point(0, 0);
            mspMenu.Name = "mspMenu";
            mspMenu.Size = new Size(876, 24);
            mspMenu.TabIndex = 17;
            mspMenu.Text = "menuStrip1";
            // 
            // btnShowFilterEditor
            // 
            btnShowFilterEditor.Cursor = Cursors.Hand;
            btnShowFilterEditor.Font = new Font("Exo 2", 11.1428566F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnShowFilterEditor.Location = new Point(693, 445);
            btnShowFilterEditor.Margin = new Padding(3, 2, 3, 2);
            btnShowFilterEditor.Name = "btnShowFilterEditor";
            btnShowFilterEditor.Size = new Size(155, 41);
            btnShowFilterEditor.TabIndex = 19;
            btnShowFilterEditor.Text = "Show filters";
            btnShowFilterEditor.UseVisualStyleBackColor = true;
            btnShowFilterEditor.Click += btnShowFilterEditor_Click;
            // 
            // chkAlwaysOnTop
            // 
            chkAlwaysOnTop.AutoSize = true;
            chkAlwaysOnTop.Location = new Point(418, 452);
            chkAlwaysOnTop.Margin = new Padding(3, 2, 3, 2);
            chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            chkAlwaysOnTop.Size = new Size(175, 34);
            chkAlwaysOnTop.TabIndex = 20;
            chkAlwaysOnTop.Text = "Always on top";
            chkAlwaysOnTop.UseVisualStyleBackColor = true;
            chkAlwaysOnTop.CheckedChanged += chkAlwaysOnTop_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(876, 539);
            Controls.Add(chkAlwaysOnTop);
            Controls.Add(btnShowFilterEditor);
            Controls.Add(chkClearLogBeforeStart);
            Controls.Add(lblStopwatch);
            Controls.Add(btnSelectFile);
            Controls.Add(lblLogFile);
            Controls.Add(txtLogFile);
            Controls.Add(grpAuthentication);
            Controls.Add(groupBox1);
            Controls.Add(stsStatus);
            Controls.Add(mspMenu);
            Controls.Add(lblServer);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(cbxServer);
            MainMenuStrip = mspMenu;
            Name = "Form1";
            Text = "XE Profiler";
            FormClosing += Form1_FormClosing;
            Move += Form1_Move;
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
        private MenuStrip mspMenu;
        private CheckBox chkClearLogBeforeStart;
        private Button btnShowFilterEditor;
        private CheckBox chkAlwaysOnTop;
    }
}
