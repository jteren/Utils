using ScreenRecorderLib;
using System.Reflection;
namespace ScreenRec
{
    public partial class Form1 : Form
    {
        private Recorder _rec;
        public Form1()
        {
            InitializeComponent();
        }

        // start program minimized to tray and create hook to start/stop recording with Pause/Break key

        private void btnStart_Click(object sender, EventArgs e)
        {            
            var options = RecorderOptions.DefaultMainMonitor;                        
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = Path.Combine(desktop, $"Capture_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.mp4");                                
            options.OutputOptions.RecorderMode = RecorderMode.Video;            
            options.OutputOptions.OutputFrameSize = new ScreenSize(2560, 1440);                        
            options.OutputOptions.Stretch = StretchMode.Uniform; // or StretchMode.Fill, StretchMode.UniformToFill                       
            options.VideoEncoderOptions.Framerate = 20;
            options.VideoEncoderOptions.Bitrate = 8_000_000; // 8 Mbps
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
            _rec.OnRecordingComplete += (s, ev) => { MessageBox.Show($"Saved: {ev.FilePath}"); };
            _rec.OnRecordingFailed += (s, ev) => { MessageBox.Show($"Recording error: {ev.Error}"); };
            _rec.Record(path);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _rec?.Stop();
        }
    }
}
