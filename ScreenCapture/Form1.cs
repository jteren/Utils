using System.Runtime.InteropServices;

namespace ScreenCapture
{
    public partial class Form1 : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        
        // Field change for thread-safety
        private static volatile bool isCapturingEnabled = true;
        private static object _captureLock = new object();



        public Form1()
        {
            InitializeComponent();
            InitializeTrayIcon();

            _proc = HookCallback;

            if (_hookID == IntPtr.Zero)
            {
                _hookID = SetHook(_proc);
            }
                        
            this.FormClosing += (s, e) => { UnhookWindowsHookEx(_hookID); _hookID = IntPtr.Zero; };
            Application.ApplicationExit += (s, e) => UnhookWindowsHookEx(_hookID);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private void InitializeTrayIcon()
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon32.ico");
            Icon customIcon = new Icon(iconPath);

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Toggle Capture", null, ToggleCapture);
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            trayIcon = new NotifyIcon
            {
                Text = "ScreenSniper",
                Icon = customIcon,
                ContextMenuStrip = trayMenu,
                Visible = true
            };
        }

        private void ToggleCapture(object sender, EventArgs e)
        {
            isCapturingEnabled = !isCapturingEnabled;
            trayIcon.BalloonTipTitle = "ScreenSniper";
            trayIcon.BalloonTipText = isCapturingEnabled ? "Capture Enabled" : "Capture Disabled";
            trayIcon.ShowBalloonTip(1000);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
                
        // Hook callback: swallow PrintScreen so OS doesn't show Snip UI
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;

            if (nCode >= 0 && isCapturingEnabled)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN && vkCode == (int)Keys.PrintScreen)
                {
                    // Handle capture on a background thread to keep hook responsive
                    Task.Run(() =>
                    {
                        // Basic re-entrancy guard so rapid presses don't overlap
                        if (!Monitor.TryEnter(_captureLock))
                            return;
                        try
                        {
                            CaptureScreen();
                        }
                        finally
                        {
                            Monitor.Exit(_captureLock);
                        }
                    });

                    // Return non-zero to prevent other apps / OS from processing PrintScreen
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void CaptureScreen()
        {
            try
            {
                var bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    string userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string folder = Path.Combine(userDesktop, "Screenshots");
                    Directory.CreateDirectory(folder);
                    string filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    string path = Path.Combine(folder, filename);
                    bmp.Save(path);
                    Console.WriteLine($"Saved: {path}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screen: {ex.Message}");
            }
        }

        // WinAPI imports
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk,
            int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
