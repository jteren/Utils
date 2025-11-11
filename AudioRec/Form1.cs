using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioRec
{
    public partial class Form1 : Form
    {
        private GlobalKeyboardHook _keyboardHook;

        private WasapiLoopbackCapture systemCapture;
        private WaveInEvent micCapture;
        private MixingSampleProvider mixer;
        private WaveFileWriter writer;
        private BufferedWaveProvider micBuffer, sysBuffer;
        public CancellationTokenSource cts;
        private Task recordTask;

        private Icon idleIcon;
        private Icon recordingIcon;


        public Form1()
        {
            InitializeComponent();
            _keyboardHook = new GlobalKeyboardHook(this);

            idleIcon = new Icon("idle.ico");         // default icon
            recordingIcon = new Icon("recording.ico"); // icon shown during recording
            notifyIcon1.Icon = idleIcon;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

        public void StartRecording()
        {
            notifyIcon1.Icon = recordingIcon;

            if (cts != null) return; // already recording

            int targetSampleRate = 44100;

            // Mic (mono) -> buffer
            micCapture = new WaveInEvent { WaveFormat = new WaveFormat(targetSampleRate, 1) };
            micBuffer = new BufferedWaveProvider(micCapture.WaveFormat) { DiscardOnBufferOverflow = true };

            // increase buffer to 10 seconds to absorb jitter
            micBuffer.BufferLength = micCapture.WaveFormat.AverageBytesPerSecond * 10;
            micCapture.DataAvailable += (s, a) => micBuffer.AddSamples(a.Buffer, 0, a.BytesRecorded);

            // System loopback -> buffer
            systemCapture = new WasapiLoopbackCapture();
            sysBuffer = new BufferedWaveProvider(systemCapture.WaveFormat) { DiscardOnBufferOverflow = true };

            // increase buffer to 10 seconds to absorb jitter
            sysBuffer.BufferLength = systemCapture.WaveFormat.AverageBytesPerSecond * 10;
            systemCapture.DataAvailable += (s, a) => sysBuffer.AddSamples(a.Buffer, 0, a.BytesRecorded);

            // Build sample providers and resample to targetSampleRate
            var micSampleProvider = micBuffer.ToSampleProvider();
            var micStereo = new MonoToStereoSampleProvider(micSampleProvider);
            var micResampled = new WdlResamplingSampleProvider(micStereo, targetSampleRate);

            var sysSampleProvider = sysBuffer.ToSampleProvider();
            var sysResampled = new WdlResamplingSampleProvider(sysSampleProvider, targetSampleRate);

            // Mixer
            mixer = new MixingSampleProvider(new[] { micResampled, sysResampled }) { ReadFully = true };

            // Convert float mixer -> 16-bit PCM provider
            var pcm16Provider = new SampleToWaveProvider16(mixer); // IWaveProvider, 16-bit PCM
            var waveFormat16 = pcm16Provider.WaveFormat; // use this for the WAV header

            // Create writer with correct 16-bit format
            writer = new WaveFileWriter(GetFilename(), waveFormat16);

            // Start captures
            cts = new CancellationTokenSource();
            systemCapture.StartRecording();
            micCapture.StartRecording();

            // Throttled write loop: read a chunk, write it, then delay according to actual audio duration.
            recordTask = Task.Run(() =>
            {
                // Use a chunk that represents e.g. 100ms of audio
                int bytesPerSecond = waveFormat16.AverageBytesPerSecond;
                int chunkMs = 100;
                int chunkBytes = Math.Max(1024, bytesPerSecond * chunkMs / 1000);
                var buffer = new byte[chunkBytes];

                try
                {
                    // raise thread priority to reduce scheduling jitter
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    long nextTargetMs = sw.ElapsedMilliseconds;
                    int flushCounter = 0;
                    while (!cts.Token.IsCancellationRequested)
                    {
                        int bytesRead = pcm16Provider.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            writer.Write(buffer, 0, bytesRead);
                            // periodically flush to minimize data loss if app crashes
                            if (++flushCounter >= 10) { writer.Flush(); flushCounter = 0; }
                            // Throttle by the real time represented by bytesRead
                            double secondsWritten = bytesRead / (double)bytesPerSecond;
                            int msSleep = (int)(secondsWritten * 1000);
                            if (msSleep < 5) msSleep = 5;
                            nextTargetMs += msSleep;
                            long delay = nextTargetMs - sw.ElapsedMilliseconds;
                            if (delay > 0)
                            {
                                try { Task.Delay((int)delay, cts.Token).Wait(cts.Token); }
                                catch (OperationCanceledException) { break; }
                            }
                            else
                            {
                                // we're behind; yield so producers can catch up
                                Thread.Yield();
                                nextTargetMs = sw.ElapsedMilliseconds;
                            }
                        }
                        else
                        {
                            // nothing available — small delay
                            try { Task.Delay(10, cts.Token).Wait(cts.Token); }
                            catch (OperationCanceledException) { break; }
                        }
                        // Optional: observe buffer sizes to detect overflow/underrun
                        // if (micBuffer.BufferedBytes > micBuffer.BufferLength * 0.9) { /* log or handle */ }
                        // if (sysBuffer.BufferedBytes > sysBuffer.BufferLength * 0.9) { /* log or handle */ }
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    BeginInvoke((Action)(() => MessageBox.Show($"Recording error: {ex.Message}")));
                }
            }, cts.Token);
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        public void StopRecording()
        {
            notifyIcon1.Icon = idleIcon;

            if (cts == null) return;

            // Signal stop
            cts.Cancel();

            // Stop captures
            try { systemCapture?.StopRecording(); } catch { }
            try { micCapture?.StopRecording(); } catch { }

            // Wait for recording task to finish
            try { recordTask?.Wait(3000); } catch { }

            // Finalize writer and dispose
            try { writer?.Flush(); writer?.Dispose(); } catch { }
            systemCapture?.Dispose();
            micCapture?.Dispose();

            string? wavPath = writer?.Filename;

            if (File.Exists(wavPath))
            {
                try { ConvertWavToMp3(wavPath); }
                catch (Exception ex)
                {
                    MessageBox.Show($"MP3 conversion failed: {ex.Message}");
                }
            }

            cts.Dispose();
            cts = null;
            recordTask = null;
            writer = null;
        }

        private string GetFilename()
        {
            string userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var ts = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return $@"{userDesktop}\TeamsCall_{ts}.wav";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;

            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, ev) => RestoreFromTray());
            trayMenu.Items.Add("Exit", null, (s, ev) => Application.Exit());

            notifyIcon1.ContextMenuStrip = trayMenu;
        }

        private void ConvertWavToMp3(string wavPath)
        {
            string mp3Path = Path.ChangeExtension(wavPath, ".mp3");
            
            using (var reader = new AudioFileReader(wavPath))
            {                

                MediaFoundationEncoder.EncodeToMp3(reader, mp3Path, 192000);
            }
            File.Delete(wavPath); // delete original WAV after successful conversion
        }

        private void RestoreFromTray()
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _keyboardHook.Unhook();
            base.OnFormClosing(e);
        }
    }
}