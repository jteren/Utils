using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLEventProfiler
{
    public partial class FilterEditorForm : Form
    {
        public string FilterText { get; private set; }
        private string originalText;

        public FilterEditorForm(string existingFilters)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.Text = "Filters"; // optional
            originalText = existingFilters;
            txtFilters.Text = existingFilters;
            ColorizeFilters();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FilterText = txtFilters.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FilterEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save filters back to main form
            if (Owner is Form1 main)
            {
                main.ResetFilterEditorButton();
                main.UpdateFiltersFromEditor(txtFilters.Text);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtFilters.Text = originalText;
            if (Owner is Form1 main)
            {
                this.Close();
                main.ResetFilterEditorButton();
            }
        }

        private void ColorizeFilters()
        {
            int selectionStart = txtFilters.SelectionStart;
            int selectionLength = txtFilters.SelectionLength;

            txtFilters.SuspendLayout();

            // Remove previous formatting
            txtFilters.SelectAll();
            txtFilters.SelectionColor = Color.Black;

            // Apply gray to lines starting with #
            var lines = txtFilters.Lines;
            int index = 0;

            foreach (var line in lines)
            {
                int lineStart = index;
                int lineLength = line.Length;

                if (line.StartsWith("#"))
                {
                    txtFilters.Select(lineStart, lineLength);
                    txtFilters.SelectionColor = Color.Gray;
                }

                // Move index to next line (include newline)
                index += lineLength + 1;
            }

            // Restore cursor
            txtFilters.SelectionStart = selectionStart;
            txtFilters.SelectionLength = selectionLength;

            txtFilters.ResumeLayout();
        }

        private void txtFilters_TextChanged(object sender, EventArgs e)
        {
            ColorizeFilters();
        }
    }
}
