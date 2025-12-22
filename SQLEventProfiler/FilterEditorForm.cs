using System.Text;

namespace SQLEventProfiler
{
    public partial class FilterEditorForm : Form
    {        
        private string originalTextExec;
        private string originalText;

        private int currentLineIndexSelect = -1;
        private int currentLineIndexExec = -1;        

        private List<string> schemaFilters = new List<string>();

        public FilterEditorForm(string existingFilters, List<string> schemas)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            
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
                else if (normalized.StartsWith("schema=", StringComparison.OrdinalIgnoreCase))
                {
                    string schemaName = normalized.Replace("schema=", "").Trim();
                    if (!string.IsNullOrWhiteSpace(schemaName))
                    {
                        schemaFilters.Add(schemaName);
                    }
                }
            }

            txtFilters.BackColor = Color.FromArgb(245, 245, 245);
            txtFiltersExec.BackColor = Color.FromArgb(245, 245, 245);
                                    
            AddCheckboxesToPanel(pnlSchemas, schemas);
            
            foreach (CheckBox cbx in pnlSchemas.Controls.OfType<CheckBox>())
            {
                if (schemaFilters.Contains(cbx.Tag.ToString()))
                {
                    cbx.Checked = true;
                }
            }

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
                    line = "# " + line;
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

        private void AddCheckboxesToPanel(Panel panel, List<string> items)
        {
            panel.Controls.Clear();
            panel.AutoScroll = true;

            int colCount = 3;
            int cbWidth = 300;
            int cbHeight = 45;
            int spacing = 5;

            Panel spacer = new Panel();
            spacer.Width = 15;
            spacer.Height = 20;
            spacer.Location = new Point(0, 0);
            panel.Controls.Add(spacer);

            for (int i = 0; i < items.Count; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Text = items[i];
                cb.Name = $"chk{items[i]}";
                cb.Tag = items[i];
                cb.AutoSize = false;
                cb.Width = 290;
                cb.Height = cb.PreferredSize.Height; // ensures correct height
                cb.AutoEllipsis = true;
                cb.UseCompatibleTextRendering = true; // <-- fixes vertical alignment

                int col = i % colCount;
                int row = i / colCount;

                cb.Left = col * (cbWidth + spacing) + spacer.Width;
                cb.Top = row * (cbHeight + spacing) + spacer.Height;
                cb.Width = cbWidth;

                panel.Controls.Add(cb);
            }
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
                line = "# " + line;

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

            foreach (CheckBox cbx in pnlSchemas.Controls.OfType<CheckBox>())
            {
                sb.AppendLine($"{(cbx.Checked ? $"schema={cbx.Tag}" : "")}");
            }

            return sb.ToString().TrimEnd();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
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
            if (Owner is Form1 main)
            {
                main.SelectedFiltersTabIndex = tabFilterCategories.SelectedIndex;
                main.ResetFilterEditorButton();                
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

        private void ColorizeFilters(RichTextBox rtb)
        {
            int selectionStart = rtb.SelectionStart;
            int selectionLength = rtb.SelectionLength;

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

        private void FilterEditorForm_Load(object sender, EventArgs e)
        {
            if (Owner is Form1 main)
            {
                tabFilterCategories.SelectedIndex = main.SelectedFiltersTabIndex;
            }

            // Defer final layout/formatting until after the form is shown so RichTextBox
            // can compute correct line heights. Doing this in BeginInvoke runs the action
            // on the message loop after initial layout/handle creation is complete.
            this.BeginInvoke((Action)(() =>
            {
                // Ensure handles are created
                _ = txtFilters.Handle;
                _ = txtFiltersExec.Handle;

                // Re-apply left padding and colorization after handles exist
                ApplyLeftPadding();
                ColorizeFilters(txtFiltersExec);
                ColorizeFilters(txtFilters);

                // Force RichTextBox to re-evaluate layout:
                // - Reassign the Font (triggers WM_SETFONT)
                // - Try to set ZoomFactor to 1.0f (forces internal reflow, ignore if not supported)
                // - Call PerformLayout/Refresh to push a layout/paint pass
                ForceRichTextBoxLayout(txtFilters);
                ForceRichTextBoxLayout(txtFiltersExec);

                // Redraw line number panels so numbers line up with final layout
                pnlLineNumbers.Invalidate();
                pnlLineNumbersExec.Invalidate();
            }));
        }

        private void ForceRichTextBoxLayout(RichTextBox rtb)
        {
            // Re-create and reassign the font to trigger a WM_SETFONT and an internal layout pass.
            var oldFont = rtb.Font;
            try
            {
                rtb.Font = new Font(oldFont.FontFamily, oldFont.Size, oldFont.Style);
            }
            catch
            {
                // ignore any font recreation issues (fallback to current font)
            }

            // Re-apply selection indent (keeps left padding consistent)
            rtb.SelectAll();
            rtb.SelectionIndent = 10;
            rtb.DeselectAll();

            // Changing ZoomFactor forces rich edit to reformat; wrap in try because some envs may throw
            try
            {
                rtb.ZoomFactor = 1.0f;
            }
            catch
            {
                // ignore if zoom is not supported / invalid
            }

            rtb.PerformLayout();
            rtb.Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int radius = 22; // match Windows 11 look
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(Width - radius, Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, Height - radius, radius, radius), 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
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
