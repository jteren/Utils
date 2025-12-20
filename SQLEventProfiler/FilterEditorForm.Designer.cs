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
            btnOK.Location = new Point(618, 334);
            btnOK.Margin = new Padding(3, 2, 3, 2);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(112, 41);
            btnOK.TabIndex = 1;
            btnOK.Text = "Apply";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Exo 2", 11.1428576F);
            btnCancel.Location = new Point(735, 334);
            btnCancel.Margin = new Padding(3, 2, 3, 2);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 41);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // txtFilters
            // 
            txtFilters.BorderStyle = BorderStyle.FixedSingle;
            txtFilters.Font = new Font("Exo 2", 11F);
            txtFilters.Location = new Point(62, 10);
            txtFilters.Margin = new Padding(9, 2, 3, 2);
            txtFilters.Name = "txtFilters";
            txtFilters.Size = new Size(805, 316);
            txtFilters.TabIndex = 3;
            txtFilters.Text = "";
            txtFilters.SelectionChanged += txtFilters_SelectionChanged;
            txtFilters.TextChanged += txtFilters_TextChanged;
            // 
            // pnlLineNumbers
            // 
            pnlLineNumbers.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlLineNumbers.ForeColor = SystemColors.ControlDark;
            pnlLineNumbers.Location = new Point(-3, 10);
            pnlLineNumbers.Margin = new Padding(3, 2, 3, 2);
            pnlLineNumbers.Name = "pnlLineNumbers";
            pnlLineNumbers.Padding = new Padding(0, 0, 4, 0);
            pnlLineNumbers.RightToLeft = RightToLeft.Yes;
            pnlLineNumbers.Size = new Size(65, 316);
            pnlLineNumbers.TabIndex = 4;
            // 
            // chkCloseOnSave
            // 
            chkCloseOnSave.AutoSize = true;
            chkCloseOnSave.Checked = true;
            chkCloseOnSave.CheckState = CheckState.Checked;
            chkCloseOnSave.Font = new Font("Exo 2", 11.1428576F);
            chkCloseOnSave.Location = new Point(70, 338);
            chkCloseOnSave.Margin = new Padding(3, 2, 3, 2);
            chkCloseOnSave.Name = "chkCloseOnSave";
            chkCloseOnSave.Size = new Size(183, 38);
            chkCloseOnSave.TabIndex = 5;
            chkCloseOnSave.Text = "Close on save";
            chkCloseOnSave.UseVisualStyleBackColor = true;
            // 
            // FilterEditorForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(876, 388);
            Controls.Add(chkCloseOnSave);
            Controls.Add(pnlLineNumbers);
            Controls.Add(txtFilters);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Margin = new Padding(3, 2, 3, 2);
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