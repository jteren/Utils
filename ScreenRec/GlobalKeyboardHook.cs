using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScreenRec
{
    public class GlobalKeyboardHook 
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        private Form1 _form1;

        public GlobalKeyboardHook(Form1 form1)
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            _form1 = form1;
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (key == Keys.Pause)
                {
                    // Queue UI work instead of doing it inline in the hook
                    try
                    {
                        _form1?.BeginInvoke(new Action(() =>
                        {
                            if (_form1.cts == null)
                                _form1.StartRecording();
                            else
                                _form1.StopRecording();
                        }));
                    }
                    catch (InvalidOperationException)
                    {
                        // Form might be closing/disposed; ignore safely
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Unhook()
        {
            if (_hookID == IntPtr.Zero) return;
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
            _proc = null; // release delegate reference if desired
        }

        #region WinAPI Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk,
            int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}
