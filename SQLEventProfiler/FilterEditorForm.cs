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
        private int currentLineIndex = -1;

        public FilterEditorForm(string existingFilters)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.Text = "Filters"; // optional
            originalText = existingFilters;
            txtFilters.Text = existingFilters;

            ApplyLeftPadding();
            ColorizeFilters();

            txtFilters.SelectionChanged += txtFilters_SelectionChanged;
            pnlLineNumbers.Paint += pnlLineNumbers_Paint;
            txtFilters.DoubleClick += txtFilters_DoubleClick;

            txtFilters.VScroll += (s, e) => pnlLineNumbers.Invalidate();
            txtFilters.TextChanged += (s, e) => pnlLineNumbers.Invalidate();
            txtFilters.Resize += (s, e) => pnlLineNumbers.Invalidate();
        }

        private void txtFilters_DoubleClick(object sender, EventArgs e)
        {            
            int lineIndex = txtFilters.GetLineFromCharIndex(txtFilters.SelectionStart);
            if (lineIndex < 0 || lineIndex >= txtFilters.Lines.Length)
                return;

            string[] lines = txtFilters.Lines;
            string line = lines[lineIndex];
                        
            if (line.StartsWith("#"))
                line = line.Substring(1);     // remove #
            else
                line = "#" + line;            // add #

            lines[lineIndex] = line;
            txtFilters.Lines = lines;
                        
            int newCharIndex = txtFilters.GetFirstCharIndexFromLine(lineIndex);
            txtFilters.SelectionStart = newCharIndex + line.Length;
            txtFilters.SelectionLength = 0;

            ApplyLeftPadding();
            ColorizeFilters();            
            pnlLineNumbers.Invalidate();
        }

        private void ApplyLeftPadding()
        {
            txtFilters.SelectAll();
            txtFilters.SelectionIndent = 10;
            txtFilters.DeselectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FilterText = txtFilters.Text;
            DialogResult = DialogResult.OK;

            if (Owner is Form1 main)
            {
                main.UpdateFiltersFromEditor(txtFilters.Text);
            }

            if (chkCloseOnSave.Checked)
            {
                Close();
            }
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

        private void pnlLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            int firstLine = txtFilters.GetLineFromCharIndex(
                txtFilters.GetCharIndexFromPosition(new Point(0, 0)));

            int lastLine = txtFilters.GetLineFromCharIndex(
                txtFilters.GetCharIndexFromPosition(new Point(0, txtFilters.Height)));

            for (int i = firstLine; i <= lastLine; i++)
            {
                int charIndex = txtFilters.GetFirstCharIndexFromLine(i);
                                
                Point pos;
                if (charIndex == -1)
                {                    
                    int lineHeight = txtFilters.Font.Height;
                    int y = (i - firstLine) * lineHeight;
                    pos = new Point(0, y);
                }
                else
                {
                    pos = txtFilters.GetPositionFromCharIndex(charIndex);
                }

                string number = (i + 1).ToString();
                SizeF size = e.Graphics.MeasureString(number, this.Font);

                float x = pnlLineNumbers.Width - size.Width - 4;

                
                if (i == currentLineIndex)
                {
                    using (Brush bg = new SolidBrush(Color.FromArgb(225, 225, 225)))
                    {
                        e.Graphics.FillRectangle(bg, new RectangleF(0, pos.Y, pnlLineNumbers.Width, size.Height));
                    }
                }
                                
                using (Brush b = new SolidBrush(pnlLineNumbers.ForeColor))
                {
                    e.Graphics.DrawString(number, this.Font, b, x, pos.Y);
                }
            }
        }
        
        private void txtFilters_SelectionChanged(object sender, EventArgs e)
        {
            currentLineIndex = txtFilters.GetLineFromCharIndex(txtFilters.SelectionStart);
            pnlLineNumbers.Invalidate(); 

        }
    }

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }
    }
}
