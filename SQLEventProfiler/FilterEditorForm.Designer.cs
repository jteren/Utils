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
            tabFilterCategories = new TabControl();
            tabExec = new TabPage();
            txtFiltersExec = new RichTextBox();
            pnlLineNumbersExec = new DoubleBufferedPanel();
            tabStatements = new TabPage();
            tabSchemas = new TabPage();
            pnlSchemas = new Panel();
            tabFilterCategories.SuspendLayout();
            tabExec.SuspendLayout();
            tabStatements.SuspendLayout();
            tabSchemas.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Cursor = Cursors.Hand;
            btnOK.Font = new Font("Exo 2", 11.1428576F);
            btnOK.Location = new Point(621, 484);
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
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Font = new Font("Exo 2", 11.1428576F);
            btnCancel.Location = new Point(739, 484);
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
            txtFilters.Location = new Point(65, 10);
            txtFilters.Margin = new Padding(9, 2, 3, 2);
            txtFilters.Name = "txtFilters";
            txtFilters.Size = new Size(775, 414);
            txtFilters.TabIndex = 3;
            txtFilters.Text = "";
            txtFilters.WordWrap = false;
            txtFilters.SelectionChanged += txtFilters_SelectionChanged;
            txtFilters.TextChanged += txtFilters_TextChanged;
            // 
            // pnlLineNumbers
            // 
            pnlLineNumbers.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlLineNumbers.ForeColor = SystemColors.ControlDark;
            pnlLineNumbers.Location = new Point(0, 10);
            pnlLineNumbers.Margin = new Padding(3, 2, 3, 2);
            pnlLineNumbers.Name = "pnlLineNumbers";
            pnlLineNumbers.Padding = new Padding(0, 0, 4, 0);
            pnlLineNumbers.RightToLeft = RightToLeft.Yes;
            pnlLineNumbers.Size = new Size(65, 413);
            pnlLineNumbers.TabIndex = 4;
            // 
            // chkCloseOnSave
            // 
            chkCloseOnSave.AutoSize = true;
            chkCloseOnSave.Checked = true;
            chkCloseOnSave.CheckState = CheckState.Checked;
            chkCloseOnSave.Font = new Font("Exo 2", 11.1428576F);
            chkCloseOnSave.Location = new Point(27, 488);
            chkCloseOnSave.Margin = new Padding(3, 2, 3, 2);
            chkCloseOnSave.Name = "chkCloseOnSave";
            chkCloseOnSave.Size = new Size(183, 38);
            chkCloseOnSave.TabIndex = 5;
            chkCloseOnSave.Text = "Close on save";
            chkCloseOnSave.UseVisualStyleBackColor = true;
            // 
            // tabFilterCategories
            // 
            tabFilterCategories.Controls.Add(tabExec);
            tabFilterCategories.Controls.Add(tabStatements);
            tabFilterCategories.Controls.Add(tabSchemas);
            tabFilterCategories.Font = new Font("Exo 2", 11.1428576F);
            tabFilterCategories.Location = new Point(10, 10);
            tabFilterCategories.Margin = new Padding(3, 2, 3, 2);
            tabFilterCategories.Name = "tabFilterCategories";
            tabFilterCategories.SelectedIndex = 0;
            tabFilterCategories.Size = new Size(846, 466);
            tabFilterCategories.TabIndex = 6;
            // 
            // tabExec
            // 
            tabExec.Controls.Add(txtFiltersExec);
            tabExec.Controls.Add(pnlLineNumbersExec);
            tabExec.Location = new Point(4, 41);
            tabExec.Margin = new Padding(3, 2, 3, 2);
            tabExec.Name = "tabExec";
            tabExec.Size = new Size(838, 421);
            tabExec.TabIndex = 2;
            tabExec.Text = "Exec";
            tabExec.UseVisualStyleBackColor = true;
            // 
            // txtFiltersExec
            // 
            txtFiltersExec.BackColor = SystemColors.Window;
            txtFiltersExec.BorderStyle = BorderStyle.FixedSingle;
            txtFiltersExec.Font = new Font("Exo 2", 11F);
            txtFiltersExec.Location = new Point(65, 10);
            txtFiltersExec.Margin = new Padding(9, 2, 3, 2);
            txtFiltersExec.Name = "txtFiltersExec";
            txtFiltersExec.Size = new Size(775, 414);
            txtFiltersExec.TabIndex = 5;
            txtFiltersExec.Text = "";
            txtFiltersExec.WordWrap = false;
            txtFiltersExec.TextChanged += txtFiltersExec_TextChanged;
            // 
            // pnlLineNumbersExec
            // 
            pnlLineNumbersExec.Font = new Font("Exo 2", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlLineNumbersExec.ForeColor = SystemColors.ControlDark;
            pnlLineNumbersExec.Location = new Point(0, 10);
            pnlLineNumbersExec.Margin = new Padding(3, 2, 3, 2);
            pnlLineNumbersExec.Name = "pnlLineNumbersExec";
            pnlLineNumbersExec.Padding = new Padding(0, 0, 4, 0);
            pnlLineNumbersExec.RightToLeft = RightToLeft.Yes;
            pnlLineNumbersExec.Size = new Size(65, 412);
            pnlLineNumbersExec.TabIndex = 6;
            // 
            // tabStatements
            // 
            tabStatements.Controls.Add(txtFilters);
            tabStatements.Controls.Add(pnlLineNumbers);
            tabStatements.Location = new Point(4, 41);
            tabStatements.Margin = new Padding(3, 2, 3, 2);
            tabStatements.Name = "tabStatements";
            tabStatements.Padding = new Padding(3, 2, 3, 2);
            tabStatements.Size = new Size(838, 421);
            tabStatements.TabIndex = 0;
            tabStatements.Text = "Select";
            tabStatements.UseVisualStyleBackColor = true;
            // 
            // tabSchemas
            // 
            tabSchemas.Controls.Add(pnlSchemas);
            tabSchemas.Location = new Point(4, 41);
            tabSchemas.Margin = new Padding(3, 2, 3, 2);
            tabSchemas.Name = "tabSchemas";
            tabSchemas.Padding = new Padding(3, 2, 3, 2);
            tabSchemas.Size = new Size(838, 421);
            tabSchemas.TabIndex = 1;
            tabSchemas.Text = "Schemas";
            tabSchemas.UseVisualStyleBackColor = true;
            // 
            // pnlSchemas
            // 
            pnlSchemas.AutoScroll = true;
            pnlSchemas.Location = new Point(5, 0);
            pnlSchemas.Margin = new Padding(3, 2, 3, 2);
            pnlSchemas.Name = "pnlSchemas";
            pnlSchemas.Size = new Size(837, 427);
            pnlSchemas.TabIndex = 0;
            // 
            // FilterEditorForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 545);
            Controls.Add(tabFilterCategories);
            Controls.Add(chkCloseOnSave);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 2, 3, 2);
            Name = "FilterEditorForm";
            FormClosing += FilterEditorForm_FormClosing;
            Load += FilterEditorForm_Load;
            tabFilterCategories.ResumeLayout(false);
            tabExec.ResumeLayout(false);
            tabStatements.ResumeLayout(false);
            tabSchemas.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnOK;
        private Button btnCancel;
        private RichTextBox txtFilters;
        private DoubleBufferedPanel pnlLineNumbers;
        private CheckBox chkCloseOnSave;
        private TabControl tabFilterCategories;
        private TabPage tabStatements;
        private TabPage tabSchemas;
        private TabPage tabExec;
        private RichTextBox txtFiltersExec;
        private DoubleBufferedPanel pnlLineNumbersExec;
        private Panel pnlSchemas;
    }
}