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
        private string originalTextExec;
        private string originalText;

        private int currentLineIndexSelect = -1;
        private int currentLineIndexExec = -1;

        public FilterEditorForm(string existingFilters)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.Text = "Filters";            

            string text = existingFilters;

            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                string normalized = line.StartsWith("# ") ? line.Substring(2) : line;
                               
                if (normalized.StartsWith("exec", StringComparison.OrdinalIgnoreCase))
                {
                    originalTextExec += line + Environment.NewLine;
                    txtFiltersExec.Text += line + Environment.NewLine;
                }
                else if (normalized.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                {
                    originalText += line + Environment.NewLine;
                    txtFilters.Text += line + Environment.NewLine;
                }
            }            

            txtFilters.BackColor = Color.FromArgb(245, 245, 245);
            txtFiltersExec.BackColor = Color.FromArgb(245, 245, 245);

            ApplyLeftPadding();
            ColorizeFilters(txtFiltersExec);
            ColorizeFilters(txtFilters);

            txtFilters.SelectionChanged += txtFilters_SelectionChanged;
            pnlLineNumbers.Paint += pnlLineNumbers_Paint;
            txtFilters.DoubleClick += txtFilters_DoubleClick;

            txtFiltersExec.SelectionChanged += (s, e) =>
            {
                currentLineIndexExec = txtFiltersExec.GetLineFromCharIndex(txtFiltersExec.SelectionStart);
                pnlLineNumbersExec.Invalidate();
            };

            pnlLineNumbersExec.Paint += (s, e) =>
            {
                DrawLineNumbers(txtFiltersExec, pnlLineNumbersExec, e, currentLineIndexExec);
            };

            txtFiltersExec.DoubleClick += (s, e) =>
            {
                int lineIndex = txtFiltersExec.GetLineFromCharIndex(txtFiltersExec.SelectionStart);
                if (lineIndex < 0 || lineIndex >= txtFiltersExec.Lines.Length)
                    return;
                string[] lines = txtFiltersExec.Lines;
                string line = lines[lineIndex];
                if (line.StartsWith("#"))
                    line = line.Substring(1).Trim();
                else
                    line = "# " + line;            // add #
                lines[lineIndex] = line;
                txtFiltersExec.Lines = lines;
                int newCharIndex = txtFiltersExec.GetFirstCharIndexFromLine(lineIndex);
                txtFiltersExec.SelectionStart = newCharIndex + line.Length;
                txtFiltersExec.SelectionLength = 0;
                ApplyLeftPadding();
                ColorizeFilters(txtFiltersExec);
                pnlLineNumbersExec.Invalidate();
            };

            txtFilters.VScroll += (s, e) => pnlLineNumbers.Invalidate();
            txtFilters.TextChanged += (s, e) => pnlLineNumbers.Invalidate();
            txtFilters.Resize += (s, e) => pnlLineNumbers.Invalidate();

            txtFiltersExec.VScroll += (s, e) => pnlLineNumbersExec.Invalidate();
            txtFiltersExec.TextChanged += (s, e) => pnlLineNumbersExec.Invalidate();
            txtFiltersExec.Resize += (s, e) => pnlLineNumbersExec.Invalidate();
        }

        private void txtFilters_DoubleClick(object sender, EventArgs e)
        {
            int lineIndex = txtFilters.GetLineFromCharIndex(txtFilters.SelectionStart);
            if (lineIndex < 0 || lineIndex >= txtFilters.Lines.Length)
                return;

            string[] lines = txtFilters.Lines;
            string line = lines[lineIndex];

            if (line.StartsWith("#"))
                line = line.Substring(1).Trim();
            else
                line = "# " + line;            // add #

            lines[lineIndex] = line;
            txtFilters.Lines = lines;

            int newCharIndex = txtFilters.GetFirstCharIndexFromLine(lineIndex);
            txtFilters.SelectionStart = newCharIndex + line.Length;
            txtFilters.SelectionLength = 0;

            ApplyLeftPadding();
            ColorizeFilters(txtFilters);
            pnlLineNumbers.Invalidate();
        }

        private void ApplyLeftPadding()
        {
            txtFilters.SelectAll();
            txtFilters.SelectionIndent = 10;
            txtFilters.DeselectAll();

            txtFiltersExec.SelectAll();
            txtFiltersExec.SelectionIndent = 10;
            txtFiltersExec.DeselectAll();
        }

        // TODO when matching against filters, ignore leading/trailing whitespace and also replace [ ] with nothing

        private string GetAllFiltersText()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(txtFilters.Text))
            {
                sb.AppendLine(txtFilters.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(txtFiltersExec.Text))
            {
                sb.AppendLine(txtFiltersExec.Text.Trim());
            }
            return sb.ToString().TrimEnd();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //FilterText = txtFilters.Text;
            //FilterText += Environment.NewLine + txtFiltersExec.Text; 

            DialogResult = DialogResult.OK;

            if (Owner is Form1 main)
            {
                main.UpdateFiltersFromEditor(GetAllFiltersText());
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
                main.UpdateFiltersFromEditor(GetAllFiltersText());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtFiltersExec.Text = originalTextExec;
            txtFilters.Text = originalText;
            if (Owner is Form1 main)
            {
                this.Close();
                main.ResetFilterEditorButton();
            }
        }

        // TODO Make this generic to apply to both txtFilters and txtFiltersExec
        private void ColorizeFilters(RichTextBox rtb)
        {
            int selectionStart = txtFilters.SelectionStart;
            int selectionLength = txtFilters.SelectionLength;

            rtb.SuspendLayout();

            // Remove previous formatting
            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;

            // Apply gray to lines starting with #
            var lines = rtb.Lines;
            int index = 0;

            foreach (var line in lines)
            {
                int lineStart = index;
                int lineLength = line.Length;

                if (line.StartsWith("#"))
                {
                    rtb.Select(lineStart, lineLength);
                    rtb.SelectionColor = Color.Gray;
                }

                // Move index to next line (include newline)
                index += lineLength + 1;
            }

            // Restore cursor
            rtb.SelectionStart = selectionStart;
            rtb.SelectionLength = selectionLength;

            rtb.ResumeLayout();
        }

        private void txtFilters_TextChanged(object sender, EventArgs e)
        {
            ColorizeFilters((RichTextBox)sender);
        }

        private void txtFiltersExec_TextChanged(object sender, EventArgs e)
        {
            ColorizeFilters((RichTextBox)sender);
        }

        private void DrawLineNumbers(RichTextBox rtb, Panel panel, PaintEventArgs e, int currentLineIndex)
        {
            int firstLine = rtb.GetLineFromCharIndex(
                rtb.GetCharIndexFromPosition(new Point(0, 0)));

            int lastLine = rtb.GetLineFromCharIndex(
                rtb.GetCharIndexFromPosition(new Point(0, rtb.Height)));

            for (int i = firstLine; i <= lastLine; i++)
            {
                int charIndex = rtb.GetFirstCharIndexFromLine(i);

                Point pos;
                if (charIndex == -1)
                {
                    int lineHeight = rtb.Font.Height;
                    int y = (i - firstLine) * lineHeight;
                    pos = new Point(0, y);
                }
                else
                {
                    pos = rtb.GetPositionFromCharIndex(charIndex);
                }

                string number = (i + 1).ToString();
                SizeF size = e.Graphics.MeasureString(number, panel.Font);

                float x = panel.Width - size.Width - 4;

                if (i == currentLineIndex)
                {
                    using (Brush bg = new SolidBrush(Color.FromArgb(225, 225, 225)))
                    {
                        e.Graphics.FillRectangle(bg, new RectangleF(0, pos.Y, panel.Width, size.Height));
                    }
                }

                using (Brush b = new SolidBrush(panel.ForeColor))
                {
                    e.Graphics.DrawString(number, panel.Font, b, x, pos.Y);
                }
            }
        }


        private void pnlLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            DrawLineNumbers(txtFilters, pnlLineNumbers, e, currentLineIndexSelect);
                                  
        }

        private void txtFilters_SelectionChanged(object sender, EventArgs e)
        {
            currentLineIndexSelect = txtFilters.GetLineFromCharIndex(txtFilters.SelectionStart);
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
