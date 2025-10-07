namespace BulkUnzipper
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            btnFiles = new System.Windows.Forms.Button();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            btnFolder = new System.Windows.Forms.Button();
            txtPath = new System.Windows.Forms.TextBox();
            btnGo = new System.Windows.Forms.Button();
            cbxTopFolder = new System.Windows.Forms.CheckBox();
            lblStatus = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Multiselect = true;
            // 
            // btnFiles
            // 
            btnFiles.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            btnFiles.Location = new System.Drawing.Point(37, 40);
            btnFiles.Name = "btnFiles";
            btnFiles.Size = new System.Drawing.Size(265, 126);
            btnFiles.TabIndex = 0;
            btnFiles.Text = "Files";
            btnFiles.UseVisualStyleBackColor = true;
            btnFiles.Click += button1_Click;
            // 
            // btnFolder
            // 
            btnFolder.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            btnFolder.Location = new System.Drawing.Point(308, 40);
            btnFolder.Name = "btnFolder";
            btnFolder.Size = new System.Drawing.Size(265, 126);
            btnFolder.TabIndex = 1;
            btnFolder.Text = "Folder";
            btnFolder.UseVisualStyleBackColor = true;
            btnFolder.Click += button2_Click;
            // 
            // txtPath
            // 
            txtPath.Location = new System.Drawing.Point(37, 302);
            txtPath.Name = "txtPath";
            txtPath.Size = new System.Drawing.Size(1162, 37);
            txtPath.TabIndex = 2;
            // 
            // btnGo
            // 
            btnGo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            btnGo.Location = new System.Drawing.Point(625, 40);
            btnGo.Name = "btnGo";
            btnGo.Size = new System.Drawing.Size(275, 126);
            btnGo.TabIndex = 3;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // cbxTopFolder
            // 
            cbxTopFolder.AutoSize = true;
            cbxTopFolder.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            cbxTopFolder.Location = new System.Drawing.Point(912, 85);
            cbxTopFolder.Name = "cbxTopFolder";
            cbxTopFolder.Size = new System.Drawing.Size(287, 40);
            cbxTopFolder.TabIndex = 4;
            cbxTopFolder.Text = "Extract to top folder";
            cbxTopFolder.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(519, 427);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(0, 30);
            lblStatus.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1240, 510);
            Controls.Add(lblStatus);
            Controls.Add(cbxTopFolder);
            Controls.Add(btnGo);
            Controls.Add(txtPath);
            Controls.Add(btnFolder);
            Controls.Add(btnFiles);
            Text = "Bulk Unzipper";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private Button btnFiles;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button btnFolder;
        private TextBox txtPath;
        private Button btnGo;
        private CheckBox cbxTopFolder;
        private Label lblStatus;
    }
}
