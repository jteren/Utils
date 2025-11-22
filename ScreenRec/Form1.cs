using ScreenRecorderLib;
using System.Reflection;
using System.Drawing;

namespace ScreenRec
{
    public partial class Form1 : Form
    {
        // NOTE: Do not declare another NotifyIcon/ContextMenuStrip here if the designer already added them.
        // The designer-generated partial class likely already contains fields named `trayIcon` and `trayMenu`.
        //private NotifyIcon trayIcon;
        //private ContextMenuStrip trayMenu;

        private GlobalKeyboardHook _keyboardHook;
        public CancellationTokenSource cts;

        private Recorder _rec;
        public Form1()
        {
            // Ensure designer components are created before we reference them
            InitializeComponent();

            // Reuse designer components if present, otherwise create them.
            if (trayMenu == null)
            {
                trayMenu = new ContextMenuStrip();
            }
            else
            {
                trayMenu.Items.Clear(); // avoid duplicate menu items
            }

            // Add only the needed menu items
            trayMenu.Items.Add("Exit", null, OnExit);

            if (trayIcon == null)
            {
                trayIcon = new NotifyIcon();
            }

            trayIcon.Text = "ScreenRec";

            // If an icon is already set by the designer (e.g. custom monitor.ico), keep it.
            // Otherwise, try to load a bundled monitor.ico or fallback to Application icon.
            if (trayIcon.Icon == null)
            {
                try
                {
                    // Try common locations: current directory or embedded resource named "monitor.ico"
                    if (File.Exists("monitor.ico"))
                    {
                        trayIcon.Icon = new Icon("monitor.ico");
                      }
                    else
                    {
                        // Fallback to default
                        trayIcon.Icon = SystemIcons.Application;
                      }
                }
                catch
                {
                    trayIcon.Icon = SystemIcons.Application;
                }
            }

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;

            // Hide form itself (start minimized to tray)
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;

            _keyboardHook = new GlobalKeyboardHook(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide(); // ensure no window appears at startup
        }

        private void OnShow(object sender, EventArgs e)
        {
            Opacity = 1;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Show();
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false; // clean up
            Application.Exit();
        }

        // start program minimized to tray and create hook to start/stop recording with Pause/Break key

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

        public void StartRecording()
        {
            if (cts != null) return; // already recording

            var options = RecorderOptions.DefaultMainMonitor;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = Path.Combine(desktop, $"Capture_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.mp4");
            options.OutputOptions.RecorderMode = RecorderMode.Video;
            options.OutputOptions.OutputFrameSize = new ScreenSize(2560, 1440);
            options.OutputOptions.Stretch = StretchMode.Uniform; // or StretchMode.Fill, StretchMode.UniformToFill                       
            options.VideoEncoderOptions.Framerate = 20;
            options.VideoEncoderOptions.Bitrate = 12_000_000; // 8 Mbps
            options.VideoEncoderOptions.IsHardwareEncodingEnabled = true;
            options.VideoEncoderOptions.Quality = 80;
            options.AudioOptions.IsAudioEnabled = true;
            options.AudioOptions.IsInputDeviceEnabled = true;
            options.AudioOptions.IsOutputDeviceEnabled = true;
            options.AudioOptions.Bitrate = AudioBitrate.bitrate_128kbps;

            void TrySetEncoderProperty(object encoder, string propName, object value)
            {
                if (encoder == null) return;
                var t = encoder.GetType();
                var p = t.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (p != null && p.CanWrite)
                {
                    // Convert.ChangeType may fail for enums, so handle enums explicitly
                    if (p.PropertyType.IsEnum)
                        p.SetValue(encoder, Enum.Parse(p.PropertyType, value.ToString()));
                    else
                        p.SetValue(encoder, Convert.ChangeType(value, p.PropertyType));
                }
            }

            TrySetEncoderProperty(options.VideoEncoderOptions.Encoder, "Profile", H264Profile.Main);
            TrySetEncoderProperty(options.VideoEncoderOptions.Encoder, "TargetWidth", 2560);
            TrySetEncoderProperty(options.VideoEncoderOptions.Encoder, "TargetHeight", 1440);

            _rec = Recorder.CreateRecorder(options);
            cts = new CancellationTokenSource();
            _rec.OnRecordingComplete += (s, ev) => { MessageBox.Show($"Saved: {ev.FilePath}"); };
            _rec.OnRecordingFailed += (s, ev) => { MessageBox.Show($"Recording error: {ev.Error}"); };

            if (!cts.Token.IsCancellationRequested)
            {
                _rec.Record(path);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        public void StopRecording()
        {
            _rec?.Stop();

            if (cts == null) return;
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _keyboardHook.Unhook();
            base.OnFormClosing(e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
