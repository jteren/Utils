namespace AudioRec
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
            mutex = new Mutex(true, "AudioRecAppMutex", out createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                // MessageBox.Show("AudioRec is already running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            // Release the mutex when the app exits
            mutex.ReleaseMutex();

        }
    }
}