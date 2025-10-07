namespace BranchMerger
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
            btnRun = new Button();
            lstOutput = new ListBox();
            btnCancel = new Button();
            btnGetBranches = new Button();
            cbxRemoteBranches = new ComboBox();
            cbxLocalBranches = new ComboBox();
            openFileDialog = new OpenFileDialog();
            txtSelectedRepo = new TextBox();
            folderBrowserDialog = new FolderBrowserDialog();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtSourceFilter = new TextBox();
            txtTargetFilter = new TextBox();
            SuspendLayout();
            // 
            // btnRun
            // 
            btnRun.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRun.Location = new Point(875, 323);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(277, 263);
            btnRun.TabIndex = 0;
            btnRun.Text = "Run Merge";
            btnRun.UseVisualStyleBackColor = true;
            // 
            // lstOutput
            // 
            lstOutput.FormattingEnabled = true;
            lstOutput.Location = new Point(63, 323);
            lstOutput.Name = "lstOutput";
            lstOutput.Size = new Size(806, 304);
            lstOutput.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(875, 585);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(277, 42);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnGetBranches
            // 
            btnGetBranches.Location = new Point(953, 51);
            btnGetBranches.Name = "btnGetBranches";
            btnGetBranches.Size = new Size(199, 40);
            btnGetBranches.TabIndex = 3;
            btnGetBranches.Text = "Get Branches";
            btnGetBranches.UseVisualStyleBackColor = true;
            btnGetBranches.Click += btnGetBranches_Click;
            // 
            // cbxRemoteBranches
            // 
            cbxRemoteBranches.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbxRemoteBranches.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbxRemoteBranches.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxRemoteBranches.FlatStyle = FlatStyle.Popup;
            cbxRemoteBranches.FormattingEnabled = true;
            cbxRemoteBranches.Location = new Point(63, 224);
            cbxRemoteBranches.Name = "cbxRemoteBranches";
            cbxRemoteBranches.Size = new Size(498, 38);
            cbxRemoteBranches.TabIndex = 4;
            cbxRemoteBranches.DropDown += cbxRemoteBranches_DropDown;
            cbxRemoteBranches.DropDownClosed += cbxRemoteBranches_DropDownClosed;
            // 
            // cbxLocalBranches
            // 
            cbxLocalBranches.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxLocalBranches.FlatStyle = FlatStyle.Popup;
            cbxLocalBranches.FormattingEnabled = true;
            cbxLocalBranches.Location = new Point(654, 224);
            cbxLocalBranches.Name = "cbxLocalBranches";
            cbxLocalBranches.Size = new Size(498, 38);
            cbxLocalBranches.TabIndex = 5;
            cbxLocalBranches.DropDown += cbxLocalBranches_DropDown;
            cbxLocalBranches.DropDownClosed += cbxLocalBranches_DropDownClosed;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // txtSelectedRepo
            // 
            txtSelectedRepo.Location = new Point(63, 52);
            txtSelectedRepo.Name = "txtSelectedRepo";
            txtSelectedRepo.Size = new Size(1089, 37);
            txtSelectedRepo.TabIndex = 6;
            txtSelectedRepo.Click += txtSelectedRepo_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(64, 19);
            label1.Name = "label1";
            label1.Size = new Size(116, 30);
            label1.TabIndex = 7;
            label1.Text = "Repository";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(64, 191);
            label2.Name = "label2";
            label2.Size = new Size(152, 30);
            label2.TabIndex = 8;
            label2.Text = "Source branch";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(654, 191);
            label3.Name = "label3";
            label3.Size = new Size(146, 30);
            label3.TabIndex = 9;
            label3.Text = "Target branch";
            // 
            // txtSourceFilter
            // 
            txtSourceFilter.Location = new Point(276, 183);
            txtSourceFilter.Name = "txtSourceFilter";
            txtSourceFilter.Size = new Size(259, 37);
            txtSourceFilter.TabIndex = 10;
            // 
            // txtTargetFilter
            // 
            txtTargetFilter.Location = new Point(868, 183);
            txtTargetFilter.Name = "txtTargetFilter";
            txtTargetFilter.Size = new Size(259, 37);
            txtTargetFilter.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1181, 659);
            Controls.Add(txtTargetFilter);
            Controls.Add(txtSourceFilter);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cbxLocalBranches);
            Controls.Add(cbxRemoteBranches);
            Controls.Add(btnGetBranches);
            Controls.Add(btnCancel);
            Controls.Add(lstOutput);
            Controls.Add(btnRun);
            Controls.Add(txtSelectedRepo);
            Name = "Form1";
            Text = "Branch merger";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRun;
        private ListBox lstOutput;
        private Button btnCancel;
        private Button btnGetBranches;
        private ComboBox cbxRemoteBranches;
        private ComboBox cbxLocalBranches;
        private OpenFileDialog openFileDialog;
        private TextBox txtSelectedRepo;
        private FolderBrowserDialog folderBrowserDialog;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtSourceFilter;
        private TextBox txtTargetFilter;
    }
}
