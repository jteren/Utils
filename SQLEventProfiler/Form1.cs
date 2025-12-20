using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.XEvent.XELite;
using System.Diagnostics;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace SQLEventProfiler
{
    public partial class Form1 : Form
    {
        private class Schema
        {
            public string MenuText { get; set; }
            public string SchemaName { get; set; }
            public bool IsIgnored { get; set; }
        }

        private ToolStripMenuItem identityItem;
        private ToolStripMenuItem agoraItem;

        private CancellationTokenSource cts;
        private Task readTask;
        private StreamWriter logWriter;

        private string localServerName = Environment.MachineName;
        private const string sessionName = "jantest_session123";

        private bool validConnection = false;
        private string connString = string.Empty;
        private string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "XE_Log.sql");

        private string filterFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "filters.txt");

        private Timer timer;
        private DateTime startTime;

        private Timer statusTimer;
        private int spinnerIndex = 0;
        private readonly string[] spinnerFrames = { "Running...", "", "Running...", "" };

        private List<string> dynamicFilters = new List<string>();

        private FilterEditorForm filterEditor;

        private List<Schema> schemas = new List<Schema>
        {
            new Schema { MenuText = "Agora", SchemaName = "Agora", IsIgnored = true },
            new Schema { MenuText = "Identity", SchemaName = "eSightIdentityServer", IsIgnored = true }

        };

        public Form1()
        {
            InitializeComponent();
            PopulateServerList();
            PopulateAuthTypes();
            SetControls();

            this.Move += Form1_Move;
            LoadFiltersFromFile();
            
            statusTimer = new Timer();
            statusTimer.Interval = 800; // ms
            statusTimer.Tick += StatusTimer_Tick;

            // Setup timer
            timer = new Timer();
            timer.Interval = 10; // 10ms
            timer.Tick += Timer_Tick;
        }

        private void LoadFiltersFromFile()
        {
            try
            {
                if (File.Exists(filterFilePath))
                {
                    dynamicFilters = File.ReadAllLines(filterFilePath)
                        .Select(line => line.Trim())
                        .Where(line => line.Length > 0)
                        .ToList();
                }
            }
            catch
            {
                dynamicFilters = new List<string>();
            }
        }

        private void SaveFiltersToFile()
        {
            try
            {
                var dir = Path.GetDirectoryName(filterFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllLines(filterFilePath, dynamicFilters);
            }
            catch
            {
                MessageBox.Show("Failed to save filters.");
            }
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            spinnerIndex = (spinnerIndex + 1) % spinnerFrames.Length;
            stsStatusLabel.Text = $" {spinnerFrames[spinnerIndex]}";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            lblStopwatch.Text = $"{elapsed:hh\\:mm\\:ss\\.ff}";
        }

        private void PopulateServerList()
        {
            cbxServer.Items.Clear();
            cbxServer.Items.Add($"{localServerName}");
            cbxServer.Items.Add("dev-test-02");
            cbxServer.Items.Add("dev-test-03");
        }

        private void PopulateAuthTypes()
        {
            cbxAuthenticationType.Items.Clear();
            //cbxAuthenticationType.Items.Add("Windows Authentication");
            cbxAuthenticationType.Items.Add("SQL Server");
        }

        private void SetControls()
        {
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("File");
            ToolStripMenuItem ignoreMenuItem = new ToolStripMenuItem("Ignore");
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Help");

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");

            identityItem = new ToolStripMenuItem("Identity");
            identityItem.CheckOnClick = true;
            identityItem.Checked = true;

            agoraItem = new ToolStripMenuItem("Agora");
            agoraItem.CheckOnClick = true;
            agoraItem.Checked = true;

            identityItem.CheckedChanged += Option_CheckedChanged;
            agoraItem.CheckedChanged += Option_CheckedChanged;

            exitItem.Click += (s, e) => this.Close();

            fileMenuItem.DropDownItems.Add(exitItem);

            ignoreMenuItem.DropDownItems.Add(identityItem);
            ignoreMenuItem.DropDownItems.Add(agoraItem);

            mspMenu.Items.Add(fileMenuItem);
            mspMenu.Items.Add(ignoreMenuItem);
            mspMenu.Items.Add(helpMenuItem);

            this.MainMenuStrip = mspMenu;
            this.Controls.Add(mspMenu);

            lblStopwatch.Visible = false;
            lblStopwatch.BackColor = ColorTranslator.FromHtml("#f9f9f9");
            lblStopwatch.ForeColor = ColorTranslator.FromHtml("#a0a0a0");
            cbxServer.SelectedIndex = 0;
            cbxThisMachine.Checked = true;
            btnStop.Enabled = false;
            txtLogFile.Text = logFile;
            chkClearLogBeforeStart.Checked = true;
            stsStatus.Padding = new Padding(20, 0, 0, 0); // left, top, right, bottom

        }

        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item != null)
            {
                string state = item.Checked ? "enabled" : "disabled";

                var schema = schemas.FirstOrDefault(s => s.MenuText == item.Text);

                if (schema != null)
                {
                    schema.IsIgnored = item.Checked;
                }

                MessageBox.Show($"{item.Text} was {state}");
            }
        }

        private string BuildConnectionString()
        {
            //return "Server=JAN-PC;Database=master;TrustServerCertificate=True;Connect Timeout=2;Trusted_Connection=True;";

            var sb = new StringBuilder();
            var server = cbxServer.SelectedItem.ToString();
            sb.Append($"Server={server};Database=master;TrustServerCertificate=True;Connect Timeout=2;");
            sb.Append($"User ID={txtUserName.Text.Trim()};Password={txtPassword.Text.Trim()};");
            return sb.ToString();
        }

        private void LockControls()
        {
            cbxServer.Enabled = false;
            txtUserToLog.Enabled = false;
            cbxThisMachine.Enabled = false;
            cbxAuthenticationType.Enabled = false;
            txtUserName.Enabled = false;
            txtPassword.Enabled = false;
            btnStart.Enabled = false;
            btnPasswordSwapper.Enabled = false;
            agoraItem.Enabled = false;
            identityItem.Enabled = false;
            chkClearLogBeforeStart.Enabled = false;
        }
        private void UnlockControls()
        {
            cbxServer.Enabled = true;
            cbxThisMachine.Enabled = true;
            cbxAuthenticationType.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            if (cbxThisMachine.Checked != true)
            {
                txtUserToLog.Enabled = true;
            }

            txtUserName.Enabled = true;
            txtPassword.Enabled = true;
            btnPasswordSwapper.Enabled = true;
            lblStopwatch.Visible = false;
            agoraItem.Enabled = true;
            identityItem.Enabled = true;
            chkClearLogBeforeStart.Enabled = true;
        }

        #region Event Handlers

        private void cbxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var server = cbxServer.SelectedItem.ToString();

            cbxAuthenticationType.SelectedIndex = 0;
            txtUserName.Enabled = true;
            txtUserName.Text = "sa";
            txtPassword.Enabled = true;
            txtPassword.Text = "Paris";
        }

        private void cbxThisMachine_CheckStateChanged(object sender, EventArgs e)
        {
            txtUserToLog.Enabled = !cbxThisMachine.Checked;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {            
            LockControls();
            stsStatusLabel.Text = " Connecting...";
            connString = BuildConnectionString();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    EnsureSessionExistsAndStarted(conn);
                    lblStopwatch.Visible = true;
                    startTime = DateTime.Now;
                    timer.Start();
                    validConnection = true;

                    var nppPath = @"C:\Program Files\Notepad++\notepad++.exe";

                    if (File.Exists(logFile))
                    {
                        if (chkClearLogBeforeStart.Checked)
                        {
                            File.WriteAllText(logFile, string.Empty);
                        }

                        if (File.Exists(nppPath))
                            Process.Start(nppPath, logFile);
                        else
                            MessageBox.Show("Notepad++ not found.");
                    }
                    else
                    {
                        MessageBox.Show("File does not exist.");
                    }

                }
            }
            catch (SqlException ex)
            {
                validConnection = false;

                if (ex.Message.Contains("error occurred while establishing a connection", StringComparison.InvariantCultureIgnoreCase))
                {
                    stsStatusLabel.Text = " Failed to connect.";
                }
                else if (ex.Message.Contains("Login failed for user", StringComparison.InvariantCultureIgnoreCase))
                {                       
                    stsStatusLabel.Text = $" {ex.Message}";
                }
                else
                {
                    stsStatusLabel.Text = " SQL Error";
                }
                SqlConnection.ClearAllPools();

                // MessageBox.Show($"Error starting session: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UnlockControls();
                return;
            }
            catch (Exception ex)
            {
                UnlockControls();
                stsStatusLabel.Text = " Failed to connect.";
                MessageBox.Show($"General error: {ex.Message}");
                return;
            }

            cts = new CancellationTokenSource();
            var xeStream = new XELiveEventStreamer(connString, $"{sessionName}");

            btnStop.Enabled = true;            
            stsStatusLabel.Text = "";// " Running... ";

            spinnerIndex = 0;
            statusTimer.Start();

            try
            {
                readTask = xeStream.ReadEventStream(
                    xevent =>
                    {
                        xevent.Fields.TryGetValue("batch_text", out var bt);
                        xevent.Fields.TryGetValue("statement", out var st);
                        xevent.Fields.TryGetValue("object_name", out var on);

                        var batchText = bt?.ToString();
                        var statementText = st?.ToString();
                        var objectName = on?.ToString();

                        var textToShow = !string.IsNullOrWhiteSpace(batchText)
                            ? batchText
                            : !string.IsNullOrWhiteSpace(statementText)
                                ? statementText
                                : objectName;

                        if (ShouldIgnore(batchText) ||
                            ShouldIgnore(statementText) ||
                            ShouldIgnore(objectName))
                        {
                            return Task.CompletedTask;
                        }

                        string logEntry =
                          $"{Environment.NewLine}-- {xevent.Timestamp:yyyy-MM-dd HH:mm:ss} | " +
                          $"{xevent.Actions.GetValueOrDefault("database_name")} | " +
                          $"{xevent.Actions.GetValueOrDefault("username")}{Environment.NewLine}" +
                          //$"{xevent.Name} | " +
                          //$"  Host: {xevent.Actions.GetValueOrDefault("client_hostname")}{Environment.NewLine}" +
                          $"  {textToShow.TrimEmptyLines()}";

                        WriteLog(logEntry);

                        return Task.CompletedTask;
                    },
                    cts.Token
                );
            }
            catch (Exception ex)
            {
                UnlockControls();
                MessageBox.Show($"Error reading event stream: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool ShouldIgnore(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            foreach (var filter in dynamicFilters.Where(line => line.Length > 0 && !line.Trim().StartsWith("#")))
            {
                if (sql.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private readonly object logLock = new object();

        private void WriteLog(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(logFile))
                {
                    return;
                }

                lock (logLock)
                {
                    if (logWriter == null)
                    {
                        var dir = Path.GetDirectoryName(logFile);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Open StreamWriter for append and use UTF8; AutoFlush ensures data is written promptly.
                        logWriter = new StreamWriter(logFile, append: true, encoding: Encoding.UTF8)
                        {
                            AutoFlush = true
                        };
                    }

                    logWriter.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                // Don't throw from logging; output to debug for diagnostics.
                System.Diagnostics.Debug.WriteLine($"WriteLog error: {ex.Message}");
            }
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            UnlockControls();
            StopSessionAndCleanup();
            statusTimer.Stop();
            timer.Stop();
            lblStopwatch.Text = "00:00:00.00";
            stsStatusLabel.Text = " Stopped";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            statusTimer.Stop();
            StopSessionAndCleanup();
        }

        private void StopSessionAndCleanup()
        {
            if (validConnection)
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    StopAndDropSession(conn);
                }
            }

            cts?.Cancel();

            logWriter?.Dispose();
            logWriter = null;
        }

        #endregion

        private void EnsureSessionExistsAndStarted(SqlConnection conn)
        {
            string userOrHostNameFilter = "";

            if (cbxThisMachine.Checked)
            {
                string machineName = Environment.MachineName;
                userOrHostNameFilter = $" AND (sqlserver.client_hostname = N'{machineName}')";
            }
            else
            {
                string userToLog = txtUserToLog.Text.Trim();
                userOrHostNameFilter = $" AND (sqlserver.username LIKE N'%{userToLog}%')";
            }


            string checkSql = $@"
            IF NOT EXISTS (SELECT * FROM sys.server_event_sessions WHERE name = '{sessionName}')
            BEGIN
                CREATE EVENT SESSION [{sessionName}]
                ON SERVER
                ADD EVENT sqlserver.sql_batch_completed
                (
                    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username, sqlserver.database_name)
                    WHERE (batch_text NOT LIKE '%sys.objects%') 
                    AND (batch_text NOT LIKE '%FROM sys.%')
                    AND (batch_text <> 'select @@trancount')                                  
                    AND (batch_text <> 'SET NOEXEC, PARSEONLY, FMTONLY OFF')
                    AND (batch_text NOT LIKE 'SET SHOWPLAN%')    
                    AND (batch_text NOT LIKE '%SERVERPROPERTY%') 
                    AND (batch_text <> 'SELECT dtb.name AS [Name], dtb.state AS [State] FROM master.sys.databases dtb')
                    AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                    AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%')
                    AND (sqlserver.client_app_name <> 'Core Microsoft SqlClient Data Provider')
                    AND (sqlserver.client_app_name <> 'SQLServerCEIP')                    
                    {userOrHostNameFilter}
                ),
                ADD EVENT sqlserver.rpc_completed
                (
                    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username, sqlserver.database_name)
                    WHERE (statement NOT LIKE '%sp_help%')         
                    AND (statement NOT LIKE '%FROM sys.%')
                    AND (statement NOT LIKE '%sp_reset_connection%')
                    AND (statement NOT LIKE '%master.sys.databases%')   
                    AND (statement NOT LIKE '%DECLARE @edition sysname%')   
                    AND (statement NOT LIKE '%SERVERPROPERTY%') 
                    AND (statement <> 'exec usp_GetSystemBranding')  
                    AND (statement NOT LIKE 'usp_GetMRIDocsConfig%')  
                    AND (statement NOT LIKE '%usp_AgoraGeneralSettings_Get%')  
                    AND (statement NOT LIKE '%usp_Branding_IsDirty%')  
                    AND (statement NOT LIKE 'exec Diagnostics.usp_Log_PageUsage%') 
                    AND (statement NOT LIKE '%usp_ProcessLoggedInUserRequest%')  
                    AND (statement NOT LIKE '%from master.sys.master_files%')                      
                    AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                    AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%') 
                    AND (sqlserver.client_app_name <> 'Core Microsoft SqlClient Data Provider')
                    AND (sqlserver.client_app_name <> 'SQLServerCEIP')
                    AND (sqlserver.client_app_name <> N'Microsoft SQL Server Management Studio')                  
                    {userOrHostNameFilter}
                )
                ADD TARGET package0.ring_buffer;
            END

            IF NOT EXISTS (SELECT * FROM sys.dm_xe_sessions WHERE name = '{sessionName}')
            BEGIN
                ALTER EVENT SESSION [{sessionName}] ON SERVER STATE = START;
            END           
            ";
            using (var cmd = new SqlCommand(checkSql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void StopAndDropSession(SqlConnection conn)
        {
            string stopSql = $@"
            IF EXISTS (SELECT * FROM sys.server_event_sessions WHERE name = '{sessionName}')
            BEGIN
                ALTER EVENT SESSION [{sessionName}] ON SERVER STATE = STOP;
                DROP EVENT SESSION [{sessionName}] ON SERVER;
            END";
            using (var cmd = new SqlCommand(stopSql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text files (*.txt)|*.txt|SQL files (*.sql)|*.sql";
                ofd.Title = "Select a file";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = ofd.FileName;
                    txtLogFile.Text = selectedFile;
                    logFile = selectedFile;
                }
            }
        }

        private void btnPasswordSwapper_Click(object sender, EventArgs e)
        {
            string currentPassword = txtPassword.Text;

            if (string.Equals(currentPassword, "Paris") || string.IsNullOrWhiteSpace(currentPassword))
            {
                txtPassword.Text = "Hoover";
            }
            else
            {
                txtPassword.Text = "Paris";
            }
        }

        private void btnShowFilterEditor_Click(object sender, EventArgs e)
        {
            if (IsEditorOpen())
            {
                btnShowFilterEditor.Text = "Show Filters";
            }
            else
            {
                btnShowFilterEditor.Text = "Hide Filters";
            }

            if (filterEditor == null || filterEditor.IsDisposed)
            {
                var existing = string.Join(Environment.NewLine, dynamicFilters);
                filterEditor = new FilterEditorForm(existing);
                                
                filterEditor.Width = this.Width;
                                
                filterEditor.StartPosition = FormStartPosition.Manual;
                filterEditor.Location = new Point(this.Left, this.Bottom - 11);
                                
                filterEditor.Owner = this;
                filterEditor.FormBorderStyle = FormBorderStyle.FixedSingle;

                filterEditor.Show();
            }
            else
            {                
                filterEditor.Close();
            }          
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (filterEditor != null && !filterEditor.IsDisposed)
            {
                filterEditor.Location = new Point(this.Left, this.Bottom - 11);
            }
        }

        public void ResetFilterEditorButton()
        {
            btnShowFilterEditor.Text = "Show Filters";
        }

        public void UpdateFiltersFromEditor(string text)
        {
            dynamicFilters = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();

            SaveFiltersToFile();
        }

        private bool IsEditorOpen()
        {
            return filterEditor != null &&
                   !filterEditor.IsDisposed &&
                   filterEditor.Visible;
        }        
    }

    public static class StringExtensions
    {
        public static string TrimEmptyLines(this string input)
        {
            if (input == null) return null;
                        
            var lines = input.Split(new[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None);

            int start = 0;
            while (start < lines.Length && string.IsNullOrWhiteSpace(lines[start]))
                start++;

            int end = lines.Length - 1;
            while (end >= start && string.IsNullOrWhiteSpace(lines[end]))
                end--;

            if (start > end) return string.Empty; 

            return string.Join(Environment.NewLine, lines.AsSpan(start, end - start + 1).ToArray());
        }
    }
}
