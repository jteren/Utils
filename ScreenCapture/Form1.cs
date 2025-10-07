using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        private static bool isCapturingEnabled = true;

        public Form1()
        {
            InitializeComponent();
            InitializeTrayIcon();

            _proc = HookCallback;
            _hookID = SetHook(_proc);
            this.FormClosing += (s, e) => UnhookWindowsHookEx(_hookID);
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

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && isCapturingEnabled) 
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.PrintScreen)
                {
                    CaptureScreen();
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
                    
                    string folder = Path.Combine("C:\\Users\\Jan\\Desktop\\", "Screenshots");
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











    
