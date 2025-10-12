using LibGit2Sharp;
using System.Diagnostics;

namespace BranchMerger
{
    public partial class Form1 : Form
    {
        private string _repoPath = @"";
        private string _masterBranch = "";
        private string _featureBranch = "";
        private CancellationTokenSource _cts;

        public Form1()
        {
            InitializeComponent();
            btnRun.Click += async (_, __) => await StartWorkflowAsync();
            btnCancel.Click += (_, __) => _cts?.Cancel();

            lstOutput.DrawMode = DrawMode.OwnerDrawFixed;
            lstOutput.DrawItem += lstOutput_DrawItem;
        }

        private void lstOutput_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Get the item text
            string text = lstOutput.Items[e.Index].ToString();

            // Choose color based on index or content
            //Color itemColor = (e.Index % 2 == 0) ? Color.Red : Color.Green;

            // Draw background
            e.DrawBackground();

            if (text.Contains("❌"))
                e.Graphics.FillRectangle(Brushes.LightCoral, e.Bounds);
            else if (text.Contains("✅"))
                e.Graphics.FillRectangle(Brushes.LightGreen, e.Bounds);
            //else if (text.Contains("⚠️"))
            //    e.Graphics.FillRectangle(Brushes.Khaki, e.Bounds);
            //else if (text.Contains("✋"))
            //    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            //else if (text.StartsWith(">>>") && text.EndsWith("<<<"))
            //    e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Font font = new Font("Segoe UI", 10, FontStyle.Bold);


            SolidBrush textBrush = new SolidBrush(Color.Black);

            // Highlight selected item
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                textBrush.Color = Color.White;
                e.Graphics.FillRectangle(Brushes.DarkSlateBlue, e.Bounds);
            }

            // Draw text
            using (Brush brush = textBrush)
            {
                e.Graphics.DrawString(text, font, brush, e.Bounds); // e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            // Draw focus rectangle if needed
            e.DrawFocusRectangle();
        }

        private async Task StartWorkflowAsync()
        {

            lstOutput.Items.Clear();

            if (!Directory.Exists(_repoPath))
            {
                AppendOutput($"❌ Repository not found");
                return;
            }
            
            _masterBranch = cbxRemoteBranches.SelectedItem?.ToString() ?? "";
            _featureBranch = cbxLocalBranches.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(_masterBranch) || string.IsNullOrWhiteSpace(_featureBranch))
            {
                AppendOutput($"❌ Please select both source and target branches.");
                return;
            }
            else if (_masterBranch.Equals(_featureBranch))
            {
                AppendOutput($"❌ Cannot merge the branch to itself.");
                return;
            }

            btnRun.Enabled = false;
            btnCancel.Enabled = true;
            lstOutput.Items.Clear();

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            var steps = new (string Name, string FileName, string Args)[]
            {
                ($"Checkout source branch",  "git", $"checkout {_masterBranch}"),
                ("Pull source branch",      "git", "pull"),
                ("Checkout target branch", "git", $"checkout {_featureBranch}"),
                ("Merge source branch",     "git", $"merge {_masterBranch}"),
                ("Push target branch",     "git", $"push origin {_featureBranch}")
            };

            try
            {
                foreach (var step in steps)
                {
                    token.ThrowIfCancellationRequested();
                    AppendOutput($">>> {step.Name.ToUpper()} <<<");

                    int code = await RunProcessAsync(
                        step.FileName,
                        step.Args,
                        _repoPath,
                        token
                    );

                    if (code != 0)
                    {
                        AppendOutput($"❌ {step.Name} failed (exit code {code})");
                        if (step.Name == "Merge master")
                            AppendOutput("⚠️ Merge conflict detected. Resolve and retry.");
                        break;
                    }

                    AppendOutput($"✅ {step.Name} succeeded");
                }
            }
            catch (OperationCanceledException)
            {
                AppendOutput("✋ Workflow cancelled by user.");
            }
            finally
            {
                btnRun.Enabled = true;
                btnCancel.Enabled = false;
                _cts.Dispose();
                _cts = null;
            }
        }

        private Task<int> RunProcessAsync(
            string fileName,
            string arguments,
            string workingDir,
            CancellationToken token)
        {
            var tcs = new TaskCompletionSource<int>();
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var proc = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            // Register cancellation *before* starting
            var registration = token.Register(() =>
            {
                try
                {
                    // Simply attempt a kill; if the process is gone or not started, Kill() will throw
                    proc.Kill();
                }
                catch (InvalidOperationException)
                {
                    // Process is already dead or not yet started—safe to ignore
                }
                catch (Exception ex)
                {
                    // Log unexpected errors if you like
                    Debug.WriteLine($"Error killing process: {ex}");
                }
            });

            proc.Exited += (_, __) =>
            {
                // Signal completion
                tcs.TrySetResult(proc.ExitCode);

                // Clean up
                registration.Dispose();
                proc.Dispose();
            };

            // Wire up output streaming
            proc.OutputDataReceived += (s, e) => { if (e.Data != null) AppendOutput(e.Data); };
            proc.ErrorDataReceived += (s, e) => { if (e.Data != null) AppendOutput(e.Data); };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            return tcs.Task;
        }

        private void AppendOutput(string text)
        {
            if (lstOutput.InvokeRequired)
            {
                lstOutput.Invoke(new Action(() => {
                    lstOutput.Items.Add(text);
                    lstOutput.TopIndex = lstOutput.Items.Count - 1; // Auto-scroll
                }));
            }
            else
            {
                lstOutput.Items.Add(text);
                lstOutput.TopIndex = lstOutput.Items.Count - 1; 
            }
        }

        private void GetBranches()
        {
            cbxLocalBranches.Items.Clear();
            cbxRemoteBranches.Items.Clear();
            lstOutput.Items.Clear();

            if (string.IsNullOrWhiteSpace(_repoPath) || !Directory.Exists(_repoPath))
            {
                AppendOutput("❌ Please select a valid repository path.");
                return;
            }

            _allRemoteBranchItems.Clear();

            try
            {
                using (var repo = new Repository(_repoPath))
                {
                    foreach (var branch in repo.Branches.Where(x => !x.IsRemote).ToList())
                    {
                        _allRemoteBranchItems.Add(branch.FriendlyName);

                        var latestCommit = branch.Tip;
                        if (latestCommit == null) continue;

                        var author = latestCommit.Author;

                        //if (author.Name.Equals(authorNameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                        //    author.Email.Equals(authorNameOrEmail, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    if (!branch.IsRemote)
                        //        cbxLocalBranches.Items.Add(branch.FriendlyName);
                        //    else
                        //        cbxRemoteBranches.Items.Add(branch.FriendlyName);
                        //}

                        if (!branch.IsRemote)
                        {
                            cbxRemoteBranches.Items.Add(branch.FriendlyName);

                            //if (string.IsNullOrWhiteSpace(txtSourceFilter.Text))
                            //{
                            //    cbxRemoteBranches.Items.Add(branch.FriendlyName);
                            //}
                            //else if (!string.IsNullOrWhiteSpace(txtSourceFilter.Text) &&
                            //          branch.FriendlyName.Contains(txtSourceFilter.Text, StringComparison.OrdinalIgnoreCase))
                            //{
                            //    cbxRemoteBranches.Items.Add(branch.FriendlyName);
                            //}
                            //else { }                       
                        }
                    }

                    foreach (var branch in repo.Branches)
                    {
                        if (!branch.IsRemote)
                        {
                            cbxLocalBranches.Items.Add(branch.FriendlyName);
                        }
                    }
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                AppendOutput($"❌ No git repository found in selected folder.");
            }
        }

        private void btnGetBranches_Click(object sender, EventArgs e)
        {
            GetBranches();
        }

        private void txtSelectedRepo_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    Properties.Settings.Default.LastFolderPath = selectedPath;
                    Properties.Settings.Default.Save();

                    txtSelectedRepo.Text = selectedPath;
                    _repoPath = selectedPath;
                    GetBranches();
                }
            }
        }

        private List<string> _allRemoteBranchItems = new(); 
        private void cbxRemoteBranches_DropDown(object sender, EventArgs e)
        {
            // Only copy once, when _allRemoteBranchItems is null or empty
            //if (_allRemoteBranchItems == null || _allRemoteBranchItems.Count == 0)
            //    _allRemoteBranchItems = cbxRemoteBranches.Items.Cast<string>().ToList();

            string filter = txtSourceFilter.Text;
            var matchingItems = cbxRemoteBranches.Items.Cast<string>()
                .Where(item => item.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            cbxRemoteBranches.Items.Clear();
            foreach (var item in matchingItems)
            {
                cbxRemoteBranches.Items.Add(item);
            }
        }

        private void cbxRemoteBranches_DropDownClosed(object sender, EventArgs e)
        {
            // Store the current selection
            var selectedItem = cbxRemoteBranches.SelectedItem;

            cbxRemoteBranches.Items.Clear();
            if (_allRemoteBranchItems != null)
            {
                foreach (var item in _allRemoteBranchItems)
                {
                    cbxRemoteBranches.Items.Add(item);
                }
            }

            // Restore the selection if possible
            if (selectedItem != null && cbxRemoteBranches.Items.Contains(selectedItem))
            {
                cbxRemoteBranches.SelectedItem = selectedItem;
            }
        }

        private void cbxLocalBranches_DropDown(object sender, EventArgs e)
        {
            string filter = txtTargetFilter.Text;
            var matchingItems = cbxLocalBranches.Items.Cast<string>()
                .Where(item => item.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            cbxLocalBranches.Items.Clear();
            foreach (var item in matchingItems)
            {
                cbxLocalBranches.Items.Add(item);
            }
        }

        private void cbxLocalBranches_DropDownClosed(object sender, EventArgs e)
        {
            // Store the current selection
            var selectedItem = cbxLocalBranches.SelectedItem;

            cbxLocalBranches.Items.Clear();
            if (_allRemoteBranchItems != null)
            {
                foreach (var item in _allRemoteBranchItems)
                {
                    cbxLocalBranches.Items.Add(item);
                }
            }

            // Restore the selection if possible
            if (selectedItem != null && cbxLocalBranches.Items.Contains(selectedItem))
            {
                cbxLocalBranches.SelectedItem = selectedItem;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int x = Properties.Settings.Default.WindowLocationX;
            int y = Properties.Settings.Default.WindowLocationY;

            // Optional: validate screen bounds
            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            if (screenBounds.Contains(new Point(x, y)))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(x, y);
            }

            string sourceFilter = Properties.Settings.Default.SourceFilter;
            if (!string.IsNullOrEmpty(sourceFilter))
            {
                txtSourceFilter.Text = sourceFilter;
            }

            string lastPath = Properties.Settings.Default.LastFolderPath;
            if (!string.IsNullOrEmpty(lastPath))
            {                
                txtSelectedRepo.Text = lastPath;
                _repoPath = lastPath;
                GetBranches();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.SourceFilter = txtSourceFilter.Text;
            Properties.Settings.Default.WindowLocationX = this.Location.X;
            Properties.Settings.Default.WindowLocationY = this.Location.Y;

            Properties.Settings.Default.Save();
        }

        private void btnSelectCurrent_Click(object sender, EventArgs e)
        {
            string repoPath = _repoPath;
            using (var repo = new Repository(_repoPath))
            {
                string branchName = repo.Head.FriendlyName;
                // pre select current branch
                cbxLocalBranches.SelectedItem = branchName;
            }
        }       
    }
}
