using Microsoft.Data.SqlClient;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace SQLEventProfiler
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts;
        private DateTime lastTimestamp = DateTime.MinValue;                   
        string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "XE_Log.txt");             
        private const string sessionName = "jantest_session";
        string connString = String.Empty;
        string localServerName = Environment.MachineName;
        bool validConnection = true;
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
            cbxServer.Items.Add("Server2");
        }

        private void PopulateAuthTypes()
        {
            cbxAuthenticationType.Items.Clear();
            cbxAuthenticationType.Items.Add("Windows Authentication");
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
            var sb = new StringBuilder();

            var server = cbxServer.SelectedItem.ToString();

            sb.Append($"Server={server};Database=master;TrustServerCertificate=True;Connect Timeout=2;");

            if (server == $"{localServerName}")
            {
                sb.Append("Integrated Security=True;");
            }
            else
            {
                sb.Append($"User ID={txtUserToLog.Text};Password={txtPassword.Text};");
            }

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

            if (cbxAuthenticationType.SelectedIndex != 0)
            {
                txtUserName.Enabled = true;
                txtPassword.Enabled = true;
            }
        }

        #region Event Handlers

        private void cbxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var server = cbxServer.SelectedItem.ToString();

            if (server == $"{localServerName}")
            {
                cbxAuthenticationType.SelectedIndex = 0;
                txtUserName.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                cbxAuthenticationType.SelectedIndex = 1;
                txtUserName.Enabled = true;
                txtUserName.Text = "sa";
                txtPassword.Enabled = true;
                txtPassword.Text = "ParXXXXX"; // or HooXXXXX
            }

            connString = BuildConnectionString();
        }

        private void cbxThisMachine_CheckStateChanged(object sender, EventArgs e)
        {
            txtUserToLog.Enabled = !cbxThisMachine.Checked;
        }

        private void cbxAuthenticationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAuthenticationType.SelectedItem.ToString() == "Windows Authentication")
            {
                txtUserName.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                txtUserName.Enabled = true;

                txtPassword.Enabled = true;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            LockControls();
            stsStatusLabel.Text = " Connecting...";

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

            btnStop.Enabled = true;

            cts = new CancellationTokenSource();
            stsStatusLabel.Text = " Running... ";

            spinnerIndex = 0;
            statusTimer.Start();

            await Task.Run(() => PollRingBuffer(cts.Token));
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
        }

        #endregion

        private void PollRingBuffer(CancellationToken token)
        {
            // ev.value('(action[@name=""client_app_name""]/value)[1]', 'nvarchar(128)') AS client_app_name
            string query = $@"
                WITH Events AS (
                    SELECT
                        ev.value('@name','varchar(50)') AS event_name,
                        ev.value('@timestamp','datetime') AS event_time,
                        ev.value('(data[@name=""batch_text""]/value)[1]','nvarchar(max)') AS batch_text,
                        ev.value('(action[@name=""database_name""]/value)[1]','nvarchar(128)') AS database_name,
                        ev.value('(action[@name=""username""]/value)[1]','nvarchar(128)') AS username,
                        ev.value('(action[@name=""client_hostname""]/value)[1]','nvarchar(128)') AS client_hostname                        
                    FROM (
                        SELECT CAST(target_data AS XML) AS td
                        FROM sys.dm_xe_session_targets st
                        JOIN sys.dm_xe_sessions s ON s.address = st.event_session_address
                        WHERE st.target_name = 'ring_buffer'
                          AND s.name = '{sessionName}'
                    ) rb
                    CROSS APPLY rb.td.nodes('//RingBufferTarget/event') AS T(ev)
                )
                SELECT TOP 20 *
                FROM Events
                ORDER BY event_time DESC;";

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                while (!token.IsCancellationRequested)
                {
                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime ts = reader.GetDateTime(reader.GetOrdinal("event_time"));
                            if (ts > lastTimestamp)
                            {
                                string line = $"{ts:yyyy-MM-dd HH:mm:ss} | {reader["database_name"]} | {reader["username"]} | {System.Environment.NewLine} {reader["batch_text"].ToString().Trim()} {System.Environment.NewLine}";
                                File.AppendAllText(logFile, line + Environment.NewLine);
                                lastTimestamp = ts;
                            }
                        }
                    }
                    Thread.Sleep(3000);
                }
            }
        }

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
                additionalFilter = $"AND (sqlserver.username = N'{userToLog}')";
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
    }
}
