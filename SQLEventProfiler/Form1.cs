using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.XEvent.XELite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace SQLEventProfiler
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts;
        private Task readTask;
        private StreamWriter logWriter;

        private string localServerName = Environment.MachineName;
        private const string sessionName = "jantest_session123";
        private const string ParisPassword = "Paris";
        private const string HooverPassword = "Hoover";
        private const string saUserName = "sa";
        private const string ShowFilters = "Show Filters";
        private const string HideFilters = "Hide Filters";
        private const string Running = "Running...";
        private const string Connecting = " Connecting...";
        private const string Stopped = " Stopped.";
        private const string FailedToConnect = " Failed to connect.";
        private const string SQLError = " SQL Error.";
        private const string SessionStoppedUnexpectedly = " Session stopped unexpectedly.";
        private const string FailedToSaveFilters = " Failed to save filters.";

        private bool validConnection = false;
        private string connString = string.Empty;
        private string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "XE_Log.sql");

        private string filterFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "filters.txt");

        private Timer sessionRunTimeTimer;
        private DateTime startTime;

        private Timer statusTimer;
        private int spinnerIndex = 0;
        private readonly string[] spinnerFrames = { Running, "", Running, "" };

        private Timer sessionCheckTimer;

        private List<string> dynamicFilters = new List<string>();

        private FilterEditorForm filterEditor;

        private List<string> schemas = new List<string>();

        public int SelectedFiltersTabIndex { get; set; } = 0;

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

            sessionRunTimeTimer = new Timer();
            sessionRunTimeTimer.Interval = 10; // 10ms
            sessionRunTimeTimer.Tick += SessionRunTimeTimer_Tick;
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
                MessageBox.Show(FailedToSaveFilters);
            }
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            spinnerIndex = (spinnerIndex + 1) % spinnerFrames.Length;
            stsStatusLabel.Text = $" {spinnerFrames[spinnerIndex]}";
        }

        private void SessionCheckTimer_Tick(object sender, EventArgs e)
        {
            string query = " SELECT [ses].[name] AS [exists]," +
                            " [xes].[name] AS [running]" +
                            " FROM [sys].[server_event_sessions] [ses] " +
                            " LEFT JOIN [sys].[dm_xe_sessions] [xes] " +
                            " ON [xes].[name] = [ses].[name] " +
                           $" WHERE [ses].[name] = '{sessionName}'";

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string? exists = reader["exists"] as string;
                        string? running = reader["running"] as string;

                        if (string.IsNullOrEmpty(exists) || string.IsNullOrEmpty(running))
                        {
                            // Session is missing or not running                            
                            sessionRunTimeTimer?.Stop();
                            statusTimer?.Stop();
                            sessionCheckTimer?.Stop();

                            stsStatusLabel.Text = SessionStoppedUnexpectedly;
                            lblStopwatch.Text = "00:00:00.00";
                            UnlockControls();

                            cts?.Cancel();
                            logWriter?.Dispose();
                            logWriter = null;
                        }
                    }
                }
            }
        }

        public async Task LoadDatabaseSchemasAsync(SqlConnection conn)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            string query = "USE [eSightFeature]; SELECT name FROM sys.schemas ORDER BY name";

            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    schemas.Clear();

                    while (await reader.ReadAsync())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            string schemaName = reader.GetString(0);
                            if (!string.IsNullOrWhiteSpace(schemaName)
                                && !schemaName.Contains("Test")
                                && !schemaName.Equals("sys")
                                && !schemaName.Equals("tSQLt")
                                && !schemaName.Equals("RedGateLocal")
                                && !schemaName.StartsWith("db_"))
                            {
                                schemas.Add(schemaName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Preserve original behavior of not surfacing errors to the user here; log for diagnostics.
                Debug.WriteLine($"LoadDatabaseSchemasAsync error: {ex.Message}");
            }
        }

        private void SessionRunTimeTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            lblStopwatch.Text = $"{elapsed:hh\\:mm\\:ss\\.ff}";
        }

        private void PopulateServerList()
        {
            cbxServer.Items.Clear();
            cbxServer.Items.Add($"{localServerName}");
            cbxServer.Items.Add("uksestdevsql01.ukest.lan");
            cbxServer.Items.Add("uksestsupsql01.ukest.lan");
            cbxServer.Items.Add(@"uksestsupsql01.ukest.lan\SQL2022");
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
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Help");

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");


            exitItem.Click += (s, e) => this.Close();

            fileMenuItem.DropDownItems.Add(exitItem);

            mspMenu.Items.Add(fileMenuItem);
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
            chkClearLogBeforeStart.Checked = false;
            chkAlwaysOnTop.Checked = true;
            stsStatus.Padding = new Padding(20, 0, 0, 0); // left, top, right, bottom
        }

        public string BuildConnectionString(bool local = false, string database = "eSightFeature")
        {
            if (local)
            {
                return $"Server={localServerName};Database={database};TrustServerCertificate=True;Connect Timeout=2;Trusted_Connection=True;";
            }

            if (string.IsNullOrWhiteSpace(cbxServer.Text))
            {
                throw new InvalidOperationException("Server name is required.");
            }

            var sb = new StringBuilder();
            var server = cbxServer.SelectedItem.ToString();

            if (server.Contains(@"2022"))
            {
                sb.Append($"Server={server},1533;");
            }
            else
            {
                sb.Append($"Server={server};");
            }

            sb.Append($"Database=master;TrustServerCertificate=True;Connect Timeout=2;");
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
            chkClearLogBeforeStart.Enabled = true;
        }

        #region Event Handlers

        private void cbxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var server = cbxServer.SelectedItem.ToString();

            cbxAuthenticationType.SelectedIndex = 0;
            txtUserName.Text = saUserName;

            if (server.Equals(localServerName)
                || server.Equals("uksestdevsql01.ukest.lan"))
            {
                txtPassword.Text = ParisPassword;
            }
            else
            {
                txtPassword.Text = HooverPassword;
            }
        }

        private void cbxThisMachine_CheckStateChanged(object sender, EventArgs e)
        {
            txtUserToLog.Enabled = !cbxThisMachine.Checked;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            LockControls();
            stsStatusLabel.Text = Connecting;
            connString = BuildConnectionString();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    EnsureSessionExistsAndStarted(conn);
                    lblStopwatch.Visible = true;
                    startTime = DateTime.Now;
                    sessionRunTimeTimer.Start();

                    sessionCheckTimer = new Timer();
                    sessionCheckTimer.Interval = 3000; // ms
                    sessionCheckTimer.Tick += SessionCheckTimer_Tick;
                    sessionCheckTimer.Start();

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
                    stsStatusLabel.Text = FailedToConnect;
                }
                else if (ex.Message.Contains("Login failed for user", StringComparison.InvariantCultureIgnoreCase))
                {
                    stsStatusLabel.Text = $" {ex.Message}";
                }
                else
                {
                    stsStatusLabel.Text = SQLError;
                }
                SqlConnection.ClearAllPools();


                UnlockControls();
                return;
            }
            catch (Exception ex)
            {
                UnlockControls();
                stsStatusLabel.Text = FailedToConnect;
                MessageBox.Show($"General error: {ex.Message}");
                return;
            }

            cts = new CancellationTokenSource();
            var xeStream = new XELiveEventStreamer(connString, $"{sessionName}");

            btnStop.Enabled = true;
            stsStatusLabel.Text = ""; // " Running... ";

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
                          $"{xevent.Actions.GetValueOrDefault("client_hostname")} | " +
                          $"{xevent.Actions.GetValueOrDefault("username")}{Environment.NewLine}" +
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

            foreach (var filter in dynamicFilters.Where(line => line.Length > 0 && !line.Trim().StartsWith("#") && !line.StartsWith("schema=", StringComparison.OrdinalIgnoreCase)))
            {
                if (sql.RemoveSquareBrackets().Contains(filter.RemoveSquareBrackets(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            // Schema based filters
            foreach (var filter in dynamicFilters.Where(line => line.StartsWith("schema=", StringComparison.OrdinalIgnoreCase)))
            {
                var schemaToFilter = filter.Substring("schema=".Length).Trim();
                if (string.IsNullOrWhiteSpace(schemaToFilter))
                    continue;
                if (sql.RemoveSquareBrackets().StartsWith($"exec {schemaToFilter}.", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
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

        // TODO add a queue for messages 
        // add button on the bottom of the filters to toggle selected rows #
        // remove duplicated rows on save
        // remove square brackets on save and order alphabetically

        private async void btnStop_Click(object sender, EventArgs e)
        {
            UnlockControls();
            StopSessionAndCleanup();
            statusTimer?.Stop();
            sessionRunTimeTimer?.Stop();
            sessionCheckTimer?.Stop();
            lblStopwatch.Text = "00:00:00.00";
            stsStatusLabel.Text = Stopped;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            statusTimer?.Stop();
            sessionRunTimeTimer?.Stop();
            sessionCheckTimer?.Stop();
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
                string userToLog = txtUserToLog.Text.Trim(); // TODO probably change this to hostname
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
                    WHERE (sqlserver.database_name <> 'tempdb')
                    AND (batch_text NOT LIKE '%sys.objects%') 
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
                    WHERE (sqlserver.database_name <> 'tempdb')
                    AND (statement NOT LIKE '%sp_help%')         
                    AND (statement NOT LIKE '%FROM sys.%')
                    AND (statement NOT LIKE '%sp_reset_connection%')
                    AND (statement NOT LIKE '%master.sys.databases%')   
                    AND (statement NOT LIKE '%DECLARE @edition sysname%')   
                    AND (statement NOT LIKE '%SERVERPROPERTY%') 
                    AND (statement <> 'exec usp_GetSystemBranding')  
                    AND (statement NOT LIKE '%usp_GetMRIDocsConfig%')  
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

            if (string.Equals(currentPassword, ParisPassword) || string.IsNullOrWhiteSpace(currentPassword))
            {
                txtPassword.Text = HooverPassword;
            }
            else
            {
                txtPassword.Text = ParisPassword;
            }
        }

        private async void btnShowFilterEditor_Click(object sender, EventArgs e)
        {
            if (IsEditorOpen())
            {
                btnShowFilterEditor.Text = ShowFilters;
            }
            else
            {
                btnShowFilterEditor.Text = HideFilters;
            }

            if (filterEditor == null || filterEditor.IsDisposed)
            {
                if (schemas.Count < 1)
                {
                    using (var conn = new SqlConnection(BuildConnectionString(local: true)))
                    {
                        await LoadDatabaseSchemasAsync(conn);
                    }
                }

                var existingFilters = string.Join(Environment.NewLine, dynamicFilters);
                filterEditor = new FilterEditorForm(existingFilters, schemas);

                filterEditor.Width = this.Width - 20;

                filterEditor.StartPosition = FormStartPosition.Manual;
                filterEditor.Location = new Point(this.Left + 10, this.Bottom - 10);

                filterEditor.Owner = this;
               // filterEditor.FormBorderStyle = FormBorderStyle.None;

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
                filterEditor.Location = new Point(this.Left + 10, this.Bottom - 10);
            }
        }

        public void ResetFilterEditorButton()
        {
            btnShowFilterEditor.Text = ShowFilters;
        }

        public void UpdateFiltersFromEditor(string text)
        {
            dynamicFilters = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList();

            //remove square brackets
            dynamicFilters = dynamicFilters
                .Select(line => line.RemoveSquareBrackets())
                .ToList();

            //sort by alphabetical order
            dynamicFilters = dynamicFilters.OrderBy(line => line, StringComparer.OrdinalIgnoreCase).ToList();

            //remove duplicates
            dynamicFilters = dynamicFilters.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            SaveFiltersToFile();
        }

        private bool IsEditorOpen()
        {
            return filterEditor != null &&
                   !filterEditor.IsDisposed &&
                   filterEditor.Visible;
        }       

        private void chkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkAlwaysOnTop.Checked;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x00020000;

                CreateParams cp = base.CreateParams;
                cp.ClassStyle &= ~CS_DROPSHADOW;  // remove shadow flag
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            const int DWMWA_NCRENDERING_POLICY = 2;
            const int DWMNCRP_DISABLED = 2;

            int attrValue = DWMNCRP_DISABLED;
            DwmSetWindowAttribute(this.Handle, DWMWA_NCRENDERING_POLICY, ref attrValue, sizeof(int));
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

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

        public static string RemoveSquareBrackets(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Replace("[", "").Replace("]", "").Trim();
        }
    }
}
