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
            groupBox1 = new GroupBox();
            txtUser = new TextBox();
            cbxThisMachine = new CheckBox();
            cbxAlwaysOnTop = new CheckBox();
            clbServers = new CheckedListBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // cbxServer
            // 
            cbxServer.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxServer.FormattingEnabled = true;
            cbxServer.Location = new Point(31, 55);
            cbxServer.Name = "cbxServer";
            cbxServer.Size = new Size(385, 38);
            cbxServer.TabIndex = 0;
            cbxServer.SelectedIndexChanged += cbxServer_SelectedIndexChanged;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(340, 212);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(196, 129);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(542, 212);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(195, 129);
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
            stsStatus.Location = new Point(0, 428);
            stsStatus.Name = "stsStatus";
            stsStatus.Size = new Size(800, 22);
            stsStatus.TabIndex = 6;
            stsStatus.Text = "statusStrip1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtUser);
            groupBox1.Controls.Add(cbxThisMachine);
            groupBox1.Location = new Point(444, 25);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(322, 132);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log for:";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(17, 79);
            txtUser.Name = "txtUser";
            txtUser.PlaceholderText = "Username to log for";
            txtUser.Size = new Size(278, 37);
            txtUser.TabIndex = 1;
            // 
            // cbxThisMachine
            // 
            cbxThisMachine.AutoSize = true;
            cbxThisMachine.Location = new Point(17, 39);
            cbxThisMachine.Name = "cbxThisMachine";
            cbxThisMachine.Size = new Size(164, 34);
            cbxThisMachine.TabIndex = 0;
            cbxThisMachine.Text = "This machine";
            cbxThisMachine.UseVisualStyleBackColor = true;
            cbxThisMachine.CheckStateChanged += cbxThisMachine_CheckStateChanged;
            // 
            // cbxAlwaysOnTop
            // 
            cbxAlwaysOnTop.AutoSize = true;
            cbxAlwaysOnTop.Location = new Point(625, 391);
            cbxAlwaysOnTop.Name = "cbxAlwaysOnTop";
            cbxAlwaysOnTop.Size = new Size(175, 34);
            cbxAlwaysOnTop.TabIndex = 8;
            cbxAlwaysOnTop.Text = "Always on top";
            cbxAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // clbServers
            // 
            clbServers.FormattingEnabled = true;
            clbServers.Location = new Point(12, 188);
            clbServers.Name = "clbServers";
            clbServers.Size = new Size(185, 140);
            clbServers.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(clbServers);
            Controls.Add(cbxAlwaysOnTop);
            Controls.Add(groupBox1);
            Controls.Add(stsStatus);
            Controls.Add(lblServer);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(cbxServer);
            Name = "Form1";
            Text = "SQL Event Profiler";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        private TextBox txtUser;
        private CheckBox cbxThisMachine;
        private CheckBox cbxAlwaysOnTop;
        private CheckedListBox clbServers;
    }
}
