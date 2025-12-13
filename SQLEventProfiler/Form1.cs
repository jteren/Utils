using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SQLEventProfiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PopulateServerList();

            // drop .xel files from previous runs
        }

        private void PopulateServerList()
        {
            cbxServer.Items.Clear();
            cbxServer.Items.Add("JAN-PC");
            cbxServer.Items.Add("Server2");

            clbServers.Items.Clear();
            clbServers.Items.Add("ServerA");
            clbServers.Items.Add("ServerB");
            clbServers.Items.Add("ServerC");
        }

        private void cbxServer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxThisMachine_CheckStateChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = !cbxThisMachine.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            RunCommandAsync("START").ConfigureAwait(false);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            RunCommandAsync("STOP").ConfigureAwait(false);
        }

        private async Task RunCommandAsync(string action)
        {
            if (cbxServer?.SelectedItem is null) 
            {
                MessageBox.Show("Please select a server.", "XE Manager - Error");
                return;                
            }

            string server = cbxServer.SelectedItem.ToString();
            string database = "master"; 
            string sessionName = "abc4"; 

            // Validate action
            if (action != "START" && action != "STOP")
                throw new ArgumentException("Invalid action.", nameof(action));

            string connString = $"Server={server};Database={database};Integrated Security=True;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Step 1: Check if session exists
                string checkSql = @"
                    SELECT COUNT(*) 
                    FROM sys.server_event_sessions 
                    WHERE name = @SessionName;";

                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@SessionName", sessionName);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists == 0)
                    {
                        // Step 2: Create session if not exists
                        string createSql = $@"
                            IF EXISTS (
                                SELECT 1 
                                FROM sys.server_event_sessions 
                                WHERE name = N'{sessionName}'
                            )
                                DROP EVENT SESSION {sessionName} ON SERVER;

                            CREATE EVENT SESSION {sessionName}
                            ON SERVER
                            ADD EVENT sqlserver.sql_batch_completed
                            (
                                ACTION (
                                    sqlserver.client_app_name,
                                    sqlserver.client_hostname,
                                    sqlserver.username,
                                    sqlserver.database_name
                                )
                                WHERE (sqlserver.client_hostname = N'{server}')
                                AND (batch_text NOT LIKE '%FROM sys.objects%')
                                AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                                AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%')
                                AND (sqlserver.client_app_name NOT LIKE '%Core Microsoft SqlClient Data Provider%')
                                AND (sqlserver.client_app_name NOT LIKE '%SQLServerCEIP%')

                            ),
                            ADD EVENT sqlserver.rpc_completed
                            (
                                ACTION (
                                    sqlserver.client_app_name,
                                    sqlserver.client_hostname,
                                    sqlserver.username,
                                    sqlserver.database_name
                                )
                                WHERE (sqlserver.client_hostname = N'{server}')
                                AND (statement NOT LIKE '%sp_help%')
                                AND (statement NOT LIKE '%sp_reset_connection%')
                                AND (statement NOT LIKE '%master.sys.databases%')
                                AND (sqlserver.client_app_name NOT LIKE '%Transact-SQL IntelliSense%')
                                AND (sqlserver.client_app_name NOT LIKE '%Red Gate Software Ltd - SQL Prompt%') 
                                AND (sqlserver.client_app_name NOT LIKE '%Core Microsoft SqlClient Data Provider%')
                                AND (sqlserver.client_app_name NOT LIKE '%SQLServerCEIP%')

                            )
                            ADD TARGET package0.event_file
                            (
                                SET filename = N'D:\{sessionName}.xel',
                                    max_file_size = 50,
                                    max_rollover_files = 5
                            );                        
                            ";

                        using (SqlCommand createCmd = new SqlCommand(createSql, conn))
                        {
                            createCmd.ExecuteNonQuery();
                        }

                        //MessageBox.Show($"Session '{sessionName}' created.", "XE Manager");
                    }


                    // Step 3: Start or stop session
                    string alterSql = $"ALTER EVENT SESSION [{sessionName}] ON SERVER STATE = {action};";
                    using (SqlCommand alterCmd = new SqlCommand(alterSql, conn))
                    {
                        await alterCmd.ExecuteNonQueryAsync();
                    }
                    
                    //MessageBox.Show($"Session '{sessionName}' started.", "XE Manager");
                }
            }

//POWERSHELL SCRIPT 
//Invoke - Sqlcmd - ServerInstance "JAN-PC" - Database "AdventureWorks2022" - TrustServerCertificate - Query "
//SELECT

// CASE
//        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'sql_batch_completed'
//            THEN ev.value('(event/data[@name=""batch_text""]/value)[1]', 'nvarchar(max)')
//        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'rpc_completed'
//            THEN 'EXEC '
//                 + ev.value('(event/data[@name=""object_name""]/value)[1]', 'nvarchar(128)')
//                 + ' '
//                 + ev.value('(event/data[@name=""statement""]/value)[1]', 'nvarchar(max)')
//        ELSE NULL
//    END AS formatted_exec,
//    ev.value('(event/@name)[1]', 'varchar(50)') AS event_name,
//    ev.value('(event/data[@name=""object_name""]/value)[1]', 'varchar(128)') AS proc_name,
//    ev.value('(event/data[@name=""batch_text""]/value)[1]', 'nvarchar(max)') AS batch_text,
//    ev.value('(event/data[@name=""duration""]/value)[1]', 'bigint') AS duration_ms,
//    ev.value('(event/action[@name=""client_app_name""]/value)[1]', 'varchar(128)') AS client_app,
//    ev.value('(event/action[@name=""client_hostname""]/value)[1]', 'varchar(128)') AS client_host,
//    ev.value('(event/action[@name=""username""]/value)[1]', 'varchar(128)') AS login_name,
//    ev.value('(event/@timestamp)[1]', 'datetime') AS event_time
//FROM sys.fn_xe_file_target_read_file('D:\abc4*.xel', NULL, NULL, NULL) AS x
//CROSS APPLY(SELECT CAST(x.event_data AS XML)) AS t(ev)
//" | Out-File "D:\hudzil131.txt"


            //try
            //{
            //    using var conn = new SqlConnection(connString);
            //    await conn.OpenAsync();
            //    string tsql = $"ALTER EVENT SESSION [{sessionName}] ON SERVER STATE = {action};";
            //    using var cmd = new SqlCommand(tsql, conn);
            //    await cmd.ExecuteNonQueryAsync();
            //    MessageBox.Show($"Session '{sessionName}' has been set to {action}.", "XE Manager");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "XE Manager - Error");
            //}
        }

    }
}


//--Create an Extended Events session to capture executions of a specific stored procedure
//CREATE EVENT SESSION [Capture_MyProc_Execs]
//ON SERVER
//ADD EVENT sqlserver.rpc_completed
//(
//    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
//    WHERE (object_name = N'MyStoredProcedure')  -- Replace with your procedure name
//)
//ADD TARGET package0.event_file
//(
//    SET filename = N'C:\XELogs\MyProcExecs.xel',  -- Replace with your desired path
//        max_file_size = 50,  -- MB
//        max_rollover_files = 5
//);

//--Start the session
//ALTER EVENT SESSION [Capture_MyProc_Execs] ON SERVER STATE = START;

//ALTER EVENT SESSION [Capture_MyProc_Execs] ON SERVER STATE = STOP;
//DROP EVENT SESSION [Capture_MyProc_Execs] ON SERVER;

//sqlcmd -S YourServerName -d YourDatabase -E -i ExportProcExecs.sql -o C:\XELogs\ProcExecs.txt

//If you want to keep a growing log:
//-With sqlcmd, use >> instead of > to append.
//- With SQL Agent, check the “Append output to existing file” option.

// This way, you’ll have a rolling text log of only the stored procedure executions you care about, without ever opening Profiler.
//Would you like me to also show you how to filter by multiple stored procedures at once (e.g., capture ProcA and ProcB together)?

/*

 Invoke-Sqlcmd -ServerInstance "YourServerName" -Database "YourDatabase" -Query "
SELECT event_data.value('(event/@name)[1]', 'varchar(50)') AS event_name,
       event_data.value('(event/data[@name=""object_name""]/value)[1]', 'varchar(128)') AS proc_name,
       event_data.value('(event/data[@name=""duration""]/value)[1]', 'bigint') AS duration_ms,
       event_data.value('(event/action[@name=""client_app_name""]/value)[1]', 'varchar(128)') AS client_app,
       event_data.value('(event/action[@name=""client_hostname""]/value)[1]', 'varchar(128)') AS client_host,
       event_data.value('(event/action[@name=""username""]/value)[1]', 'varchar(128)') AS login_name,
       event_data.value('(event/@timestamp)[1]', 'datetime') AS event_time
FROM sys.fn_xe_file_target_read_file('C:\XELogs\MyProcExecs*.xel', NULL, NULL, NULL) AS x
CROSS APPLY x.event_data.nodes('//event') AS t(event_data)
" | Out-File "C:\XELogs\ProcExecs.txt"


CREATE EVENT SESSION [Capture_MyProc_Execs]
ON SERVER
ADD EVENT sqlserver.rpc_completed
(
    WHERE (object_name = N'MyStoredProcedure')
)
ADD EVENT sqlserver.sql_batch_completed
(
    WHERE (sqlserver.sql_text LIKE '%MyStoredProcedure%')
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\MyProcExecs.xel'
);

Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "AdventureWorks2022" -TrustServerCertificate `
-Query "SELECT TOP 10 Name FROM Production.Product"

Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "AdventureWorks2022" -TrustServerCertificate `
-InputFile "C:\XELogs\ExportProcExecs.sql" -OutputFile "C:\XELogs\ProcExecs.txt"

Since you’re reading .xel files with a long query, the cleanest way is:
- Save the query into ExportProcExecs.sql.
- Run:
Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "AdventureWorks2022" `  -- can be master too
-TrustServerCertificate -InputFile "C:\XELogs\ExportProcExecs.sql" `
-OutputFile "C:\XELogs\ProcExecs.txt"

ev.value('(event/data[@name="batch_text"]/value)[1]', 'nvarchar(max)') AS batch_text

Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "AdventureWorks2022" -TrustServerCertificate -InputFile "D:\ExportProcExecs.sql" -OutputFile "D:\hudzil.txt"



-- Create a session that captures both RPC and batch executions
CREATE EVENT SESSION [Capture_Proc_Execs]
ON SERVER
ADD EVENT sqlserver.rpc_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (object_name = N'MyStoredProcedure')  -- Replace with your procedure name
)
ADD EVENT sqlserver.sql_batch_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (sqlserver.sql_text LIKE '%MyStoredProcedure%') -- Captures EXEC calls in SSMS
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\ProcExecs.xel',
        max_file_size = 50,
        max_rollover_files = 5
);

-- Start the session
ALTER EVENT SESSION [Capture_Proc_Execs] ON SERVER STATE = START;



-- Read XE file and extract both batch_text and RPC details
SELECT 
    ev.value('(event/@name)[1]', 'varchar(50)') AS event_name,
    ev.value('(event/data[@name="object_name"]/value)[1]', 'varchar(128)') AS proc_name,
    ev.value('(event/data[@name="batch_text"]/value)[1]', 'nvarchar(max)') AS batch_text,
    ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)') AS rpc_statement,
    ev.value('(event/data[@name="duration"]/value)[1]', 'bigint') AS duration_ms,
    ev.value('(event/action[@name="client_app_name"]/value)[1]', 'varchar(128)') AS client_app,
    ev.value('(event/action[@name="client_hostname"]/value)[1]', 'varchar(128)') AS client_host,
    ev.value('(event/action[@name="username"]/value)[1]', 'varchar(128)') AS login_name,
    ev.value('(event/@timestamp)[1]', 'datetime') AS event_time
FROM sys.fn_xe_file_target_read_file('C:\XELogs\ProcExecs*.xel', NULL, NULL, NULL) AS x
CROSS APPLY (SELECT CAST(x.event_data AS XML)) AS t(ev);


SELECT 
    ev.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(128)') AS proc_name,
    ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)') AS rpc_statement,
    'EXEC ' + ev.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(128)')
           + ' ' + ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)') AS formatted_exec,
    ev.value('(event/@timestamp)[1]', 'datetime') AS event_time
FROM sys.fn_xe_file_target_read_file('C:\XELogs\ProcExecs*.xel', NULL, NULL, NULL) AS x
CROSS APPLY (SELECT CAST(x.event_data AS XML)) AS t(ev);


SELECT 
    CASE 
        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'sql_batch_completed'
            THEN ev.value('(event/data[@name="batch_text"]/value)[1]', 'nvarchar(max)')
        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'rpc_completed'
            THEN 'EXEC ' 
                 + ev.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(128)')
                 + ' ' 
                 + ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)')
        ELSE NULL
    END AS formatted_exec,
    ev.value('(event/@name)[1]', 'varchar(50)') AS event_type,
    ev.value('(event/@timestamp)[1]', 'datetime') AS event_time,
    ev.value('(event/action[@name="client_app_name"]/value)[1]', 'varchar(128)') AS client_app,
    ev.value('(event/action[@name="client_hostname"]/value)[1]', 'varchar(128)') AS client_host,
    ev.value('(event/action[@name="username"]/value)[1]', 'varchar(128)') AS login_name,
    ev.value('(event/data[@name="duration"]/value)[1]', 'bigint') AS duration_ms
FROM sys.fn_xe_file_target_read_file('C:\XELogs\ProcExecs*.xel', NULL, NULL, NULL) AS x
CROSS APPLY (SELECT CAST(x.event_data AS XML)) AS t(ev);


Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "AdventureWorks2022" -TrustServerCertificate `
-InputFile "C:\XELogs\UnifiedProcExecs.sql" -OutputFile "C:\XELogs\ProcExecs.txt"


CREATE EVENT SESSION [Capture_Proc_Execs]
ON SERVER
ADD EVENT sqlserver.sql_batch_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (sqlserver.sql_text NOT LIKE '%SELECT COUNT(*) FROM sys.objects%')
)
ADD EVENT sqlserver.rpc_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (object_name = N'MyStoredProcedure')
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\ProcExecs.xel',
        max_file_size = 50,
        max_rollover_files = 5
);

ALTER EVENT SESSION [Capture_Proc_Execs] ON SERVER STATE = START;


# Start the session
Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "master" -TrustServerCertificate `
-Query "ALTER EVENT SESSION [Capture_Proc_Execs] ON SERVER STATE = START;"

# Stop the session
Invoke-Sqlcmd -ServerInstance "JAN-PC" -Database "master" -TrustServerCertificate `
-Query "ALTER EVENT SESSION [Capture_Proc_Execs] ON SERVER STATE = STOP;"


using (var cmd = new SqlCommand("ALTER EVENT SESSION [Capture_Proc_Execs] ON SERVER STATE = START;", conn))
{
    cmd.ExecuteNonQuery();
}


CREATE EVENT SESSION [Capture_MyExecs]
ON SERVER
ADD EVENT sqlserver.sql_batch_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (sqlserver.username = N'Jan')   -- replace with your SQL login
)
ADD EVENT sqlserver.rpc_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (sqlserver.username = N'Jan')
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\MyExecs.xel'
);

WHERE (sqlserver.client_hostname = N'JAN-PC')




# Create a new XE session remotely
Invoke-Sqlcmd -ServerInstance "DEV-SERVER" -Database "master" `
-Username "YourLogin" -Password "YourPassword" -TrustServerCertificate `
-Query "
CREATE EVENT SESSION [Capture_MyExecs]
ON SERVER
ADD EVENT sqlserver.sql_batch_completed
(
    ACTION (sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.username)
    WHERE (sqlserver.client_hostname = N'JAN-PC')
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\MyExecs.xel'
);
ALTER EVENT SESSION [Capture_MyExecs] ON SERVER STATE = START;
"


param(
    [string]$ServerInstance = "DEV-SERVER",   # change to your server name
    [string]$Database = "master",
    [string]$SessionName = "Capture_MyExecs",
    [ValidateSet("Start","Stop")]
    [string]$Action = "Start",
    [string]$Username = "YourLogin",          # SQL login if using SQL auth
    [string]$Password = "YourPassword"        # SQL password if using SQL auth
)

# Build the T-SQL command
$tsql = "ALTER EVENT SESSION [$SessionName] ON SERVER STATE = $Action;"

# Run it
Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database `
    -Username $Username -Password $Password -TrustServerCertificate `
    -Query $tsql

Write-Host "Event session '$SessionName' has been set to $Action."




.\ManageXESession.ps1 -ServerInstance "DEV-SERVER" -SessionName "Capture_MyExecs" -Action Start
.\ManageXESession.ps1 -ServerInstance "DEV-SERVER" -SessionName "Capture_MyExecs" -Action Stop



CREATE EVENT SESSION [Capture_Jan_Execs]
ON SERVER
ADD EVENT sqlserver.sql_batch_completed
(
    ACTION (
        sqlserver.client_app_name,
        sqlserver.client_hostname,
        sqlserver.username,
        sqlserver.database_id,
        sqlserver.database_name
    )
    WHERE (sqlserver.client_hostname = N'JAN-PC')  -- filter by your machine
)
ADD EVENT sqlserver.rpc_completed
(
    ACTION (
        sqlserver.client_app_name,
        sqlserver.client_hostname,
        sqlserver.username,
        sqlserver.database_id,
        sqlserver.database_name
    )
    WHERE (sqlserver.client_hostname = N'JAN-PC')  -- same filter
)
ADD TARGET package0.event_file
(
    SET filename = N'C:\XELogs\JanExecs.xel',
        max_file_size = 50,
        max_rollover_files = 5
);

-- Start the session
ALTER EVENT SESSION [Capture_Jan_Execs] ON SERVER STATE = START;





SELECT 
    CASE 
        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'sql_batch_completed'
            THEN ev.value('(event/data[@name="batch_text"]/value)[1]', 'nvarchar(max)')
        WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'rpc_completed'
            THEN 'EXEC ' 
                 + ev.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(128)')
                 + ' ' 
                 + ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)')
        ELSE NULL
    END AS formatted_exec,
    ev.value('(event/data[@name="database_name"]/value)[1]', 'nvarchar(128)') AS database_name,
    ev.value('(event/@timestamp)[1]', 'datetime') AS event_time,
    ev.value('(event/action[@name="client_app_name"]/value)[1]', 'varchar(128)') AS client_app,
    ev.value('(event/action[@name="client_hostname"]/value)[1]', 'varchar(128)') AS client_host,
    ev.value('(event/action[@name="username"]/value)[1]', 'varchar(128)') AS login_name
FROM sys.fn_xe_file_target_read_file('C:\XELogs\JanExecs*.xel', NULL, NULL, NULL) AS x
CROSS APPLY (SELECT CAST(x.event_data AS XML)) AS t(ev);



param(
    [string]$XelPath = "C:\XELogs\JanExecs*.xel",
    [string]$OutputFile = "C:\XELogs\JanExecs.txt",
    [int]$IntervalSeconds = 2,
    [string]$ServerInstance = "DEV-SERVER"
)

Write-Host "Monitoring XE file $XelPath and writing new events to $OutputFile..."

# Track the last timestamp we wrote
$lastTimestamp = Get-Date "1900-01-01"

while ($true) {
    try {
        # Query XE file for events newer than lastTimestamp
        $query = @"
        SELECT 
            CASE 
                WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'sql_batch_completed'
                    THEN ev.value('(event/data[@name="batch_text"]/value)[1]', 'nvarchar(max)')
                WHEN ev.value('(event/@name)[1]', 'varchar(50)') = 'rpc_completed'
                    THEN 'EXEC ' 
                         + ev.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(128)')
                         + ' ' 
                         + ev.value('(event/data[@name="statement"]/value)[1]', 'nvarchar(max)')
            END AS formatted_exec,
            ev.value('(event/data[@name="database_name"]/value)[1]', 'nvarchar(128)') AS database_name,
            ev.value('(event/@timestamp)[1]', 'datetime') AS event_time
        FROM sys.fn_xe_file_target_read_file('$XelPath', NULL, NULL, NULL) AS x
        CROSS APPLY (SELECT CAST(x.event_data AS XML)) AS t(ev)
        WHERE ev.value('(event/@timestamp)[1]', 'datetime') > '$lastTimestamp'
        ORDER BY event_time;
"@

        $events = Invoke-Sqlcmd -ServerInstance $ServerInstance -Database "master" `
            -TrustServerCertificate -Query $query

        foreach ($row in $events) {
            $line = "{0:yyyy-MM-dd HH:mm:ss} | {1} | {2}" -f $row.event_time, $row.database_name, $row.formatted_exec
            Add-Content -Path $OutputFile -Value $line

            # Update lastTimestamp to the newest event we wrote
            if ($row.event_time -gt $lastTimestamp) {
                $lastTimestamp = $row.event_time
            }
        }
    }
    catch {
        Write-Host "Error reading XE file: $_"
    }

    Start-Sleep -Seconds $IntervalSeconds
}


private void btnRunScript_Click(object sender, EventArgs e)
{
    string scriptPath = @"C:\XELogs\TailXel.ps1";
    Process.Start("powershell.exe", $"-ExecutionPolicy Bypass -File \"{scriptPath}\"");
}

  
*/




