using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.XEvent.XELite;
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

        private bool validConnection = true;
        private string connString = String.Empty;
        private string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "XE_Log.sql");

        private Timer statusTimer;
        private int spinnerIndex = 0;
        private readonly string[] spinnerFrames = { "", ">", ">>", ">>>", ">>>>" };

        public Form1()
        {
            InitializeComponent();
            PopulateServerList();
            PopulateAuthTypes();
            SetControls();
            connString = BuildConnectionString();

            statusTimer = new Timer();
            statusTimer.Interval = 800; // ms
            statusTimer.Tick += StatusTimer_Tick;
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            spinnerIndex = (spinnerIndex + 1) % spinnerFrames.Length;
            stsStatusLabel.Text = $" Running... {spinnerFrames[spinnerIndex]}";
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
            cbxServer.SelectedIndex = 0;
            cbxThisMachine.Checked = true;
            btnStop.Enabled = false;
            txtLogFile.Text = logFile;
        }

        private string BuildConnectionString()
        {
            return "Server=JAN-PC;Database=master;TrustServerCertificate=True;Connect Timeout=2;Trusted_Connection=True;";

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

        }

        #region Event Handlers

        private void cbxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var server = cbxServer.SelectedItem.ToString();

            cbxAuthenticationType.SelectedIndex = 0;
            txtUserName.Enabled = true;
            txtUserName.Text = "sa";
            txtPassword.Enabled = true;
            txtPassword.Text = "ParisXXXXX";

            connString = BuildConnectionString();
        }

        private void cbxThisMachine_CheckStateChanged(object sender, EventArgs e)
        {
            txtUserToLog.Enabled = !cbxThisMachine.Checked;
        }

        private void cbxAuthenticationType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            LockControls();
            stsStatusLabel.Text = " Connecting...";

            string connectionString =
                "Server=JAN-PC;Database=master;TrustServerCertificate=True;Connect Timeout=2;Trusted_Connection=True;";

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    EnsureSessionExistsAndStarted(conn);
                }
            }
            catch (SqlException ex)
            {
                validConnection = false;

                if (ex.Message.Contains("error occurred while establishing a connection"))
                {
                    stsStatusLabel.Text = " Failed to connect.";
                }
                SqlConnection.ClearAllPools();

                MessageBox.Show($"Error starting session: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UnlockControls();
                return;
            }
            catch (Exception ex)
            {
                UnlockControls();
                MessageBox.Show($"General error: {ex.Message}");
                return;
            }

            cts = new CancellationTokenSource();
            var xeStream = new XELiveEventStreamer(connectionString, $"{sessionName}");

            btnStop.Enabled = true;
            stsStatusLabel.Text = " Running... ";

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
            stsStatusLabel.Text = "  Stopped";
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
            string additionalFilter = "";

            if (cbxThisMachine.Checked)
            {
                string machineName = Environment.MachineName;
                additionalFilter = $"AND (sqlserver.client_hostname = N'{machineName}')";
            }
            else
            {
                string userToLog = txtUserToLog.Text.Trim();
                additionalFilter = $"AND (sqlserver.username LIKE N'%{userToLog}%')";
            }

            string checkSql = $@"
            IF NOT EXISTS (SELECT * FROM sys.server_event_sessions WHERE name = '{sessionName}')
            BEGIN
                CREATE EVENT SESSION [{sessionName}]
                ON SERVER
                ADD EVENT sqlserver.sql_batch_completed
                (
                    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username, sqlserver.database_name)
                    WHERE (batch_text NOT LIKE '%FROM sys.objects%')
                    AND (batch_text <> 'select @@trancount')
                    AND (batch_text <> 'SET NOEXEC, PARSEONLY, FMTONLY OFF')
                    AND (batch_text NOT LIKE 'SET SHOWPLAN%')                    
                    AND (batch_text <> 'SELECT dtb.name AS [Name], dtb.state AS [State] FROM master.sys.databases dtb')
                    AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                    AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%')
                    AND (sqlserver.client_app_name <> 'Core Microsoft SqlClient Data Provider')
                    AND (sqlserver.client_app_name <> 'SQLServerCEIP')
                    {additionalFilter}
                ),
                ADD EVENT sqlserver.rpc_completed
                (
                    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username, sqlserver.database_name)
                    WHERE (statement NOT LIKE '%sp_help%')
                    AND (statement NOT LIKE '%sp_reset_connection%')
                    AND (statement NOT LIKE '%master.sys.databases%')   
                    AND (statement NOT LIKE '%DECLARE @edition sysname%')   
                    AND (statement NOT LIKE '%SERVERPROPERTY%')   
                    AND (statement NOT LIKE '%from master.sys.master_files%')   
                    AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                    AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%') 
                    AND (sqlserver.client_app_name <> 'Core Microsoft SqlClient Data Provider')
                    AND (sqlserver.client_app_name <> 'SQLServerCEIP')
                    AND (sqlserver.client_app_name <> N'Microsoft SQL Server Management Studio')     
                    {additionalFilter}
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

            if (string.Equals(currentPassword, "ParisXXXXX") || string.IsNullOrWhiteSpace(currentPassword))
            {
                txtPassword.Text = "Hoover";
            }
            else
            {
                txtPassword.Text = "ParisXXXXX";
            }
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
