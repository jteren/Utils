using SQLEventProfiler;

namespace SQLEventProfiler
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;

            using (Mutex mutex = new Mutex(true, "Global\\SQLEventProfilerMutex", out createdNew))
            {
                if (!createdNew)
                {
                    // Another instance is already running
                    MessageBox.Show("Application is already running.");
                    return;
                }
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}