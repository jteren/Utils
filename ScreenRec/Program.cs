using ScreenRec;
using System.Diagnostics.Metrics;
using System.Threading;

namespace ScreenRec
{
    internal static class Program
    {
        private static Mutex mutex;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            mutex = new Mutex(true, "SRMutex", out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                // MessageBox.Show("SR is already running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            // Release the mutex when the app exits
            mutex.ReleaseMutex();
        }
    }
}