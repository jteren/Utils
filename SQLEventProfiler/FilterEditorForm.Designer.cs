namespace SQLEventProfiler
{
    partial class FilterEditorForm
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
            btnOK = new Button();
            btnCancel = new Button();
            txtFilters = new RichTextBox();
            pnlLineNumbers = new DoubleBufferedPanel();
            chkCloseOnSave = new CheckBox();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Font = new Font("Exo 2", 11.1428576F);
            btnOK.Location = new Point(721, 401);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(131, 49);
            btnOK.TabIndex = 1;
            btnOK.Text = "Apply";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Exo 2", 11.1428576F);
            btnCancel.Location = new Point(858, 401);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(131, 49);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // txtFilters
            // 
            txtFilters.BorderStyle = BorderStyle.FixedSingle;
            txtFilters.Font = new Font("Segoe UI", 10F);
            txtFilters.Location = new Point(72, 12);
            txtFilters.Margin = new Padding(10, 3, 3, 3);
            txtFilters.Name = "txtFilters";
            txtFilters.Size = new Size(938, 379);
            txtFilters.TabIndex = 3;
            txtFilters.Text = "";
            txtFilters.SelectionChanged += txtFilters_SelectionChanged;
            txtFilters.TextChanged += txtFilters_TextChanged;
            // 
            // pnlLineNumbers
            // 
            pnlLineNumbers.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlLineNumbers.ForeColor = SystemColors.ControlDark;
            pnlLineNumbers.Location = new Point(-3, 12);
            pnlLineNumbers.Name = "pnlLineNumbers";
            pnlLineNumbers.Padding = new Padding(0, 0, 5, 0);
            pnlLineNumbers.RightToLeft = RightToLeft.Yes;
            pnlLineNumbers.Size = new Size(76, 379);
            pnlLineNumbers.TabIndex = 4;
            // 
            // chkCloseOnSave
            // 
            chkCloseOnSave.AutoSize = true;
            chkCloseOnSave.Checked = true;
            chkCloseOnSave.CheckState = CheckState.Checked;
            chkCloseOnSave.Font = new Font("Exo 2", 11.1428576F);
            chkCloseOnSave.Location = new Point(82, 406);
            chkCloseOnSave.Name = "chkCloseOnSave";
            chkCloseOnSave.Size = new Size(209, 43);
            chkCloseOnSave.TabIndex = 5;
            chkCloseOnSave.Text = "Close on save";
            chkCloseOnSave.UseVisualStyleBackColor = true;
            // 
            // FilterEditorForm
            // 
            AutoScaleDimensions = new SizeF(14F, 36F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1022, 465);
            Controls.Add(chkCloseOnSave);
            Controls.Add(pnlLineNumbers);
            Controls.Add(txtFilters);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Name = "FilterEditorForm";
            Text = "Filter editor";
            FormClosing += FilterEditorForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnOK;
        private Button btnCancel;
        private RichTextBox txtFilters;
        private DoubleBufferedPanel pnlLineNumbers;
        private CheckBox chkCloseOnSave;
    }
}