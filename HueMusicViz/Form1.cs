using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.SampleConverter;
using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsVisualization.Visualization;

namespace HueMusicViz
{
    public partial class Form1 : Form
    {
        private HueClient _hueClient;

        private LineSpectrum _lineSpectrum;
        private Size size = new Size(100, 100);
        private Timer _timer = new Timer();
        private IWaveSource _source;

        private double maxIntensity;
        private double currentTopIntensity;
        private double currentTopIntensityIndex;
        private DateTime lastUpdatedTime;
        private int UPDATE_TIMER_NORMAL = 400;
        private int UPDATE_TIMER_FAST = 100;
        private int UPDATE_TIMER = 400;

        static List<String> lights = new List<String> { "1", "4" };
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setup();
        }

        private void setup()
        {
            string bridgeIp = "192.168.1.17";
            _hueClient = new HueClient(bridgeIp);
            _hueClient.Initialize("kasra-hue-music-user");

            WasapiCapture capture = new WasapiLoopbackCapture();
            capture.Initialize();

            capture.Start();
            var soundIn = new SoundInSource(capture);

            const FftSize fftSize = FftSize.Fft4096;
            var spectrumProvider = new BasicSpectrumProvider(capture.WaveFormat.Channels, capture.WaveFormat.SampleRate, fftSize);

            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 12,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Decibel
            };

            _timer.Interval = 40;
            _timer.Tick += new System.EventHandler(timer_tick);

            var notificationSource = new SingleBlockNotificationStream(soundIn.ToSampleSource());
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            _source = notificationSource.ToWaveSource(16);
            
            _timer.Start();
            capture.DataAvailable += capture_DataAvailable;

            // Turn the lights on so we don't have to keep sending ON in every command
            var command = new LightCommand().TurnOn();
            _hueClient.SendCommandAsync(command, lights);
        }

        void capture_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            _source.Read(e.Data, e.Offset, e.ByteCount);
        }

        private void timer_tick(object sender, EventArgs e)
        {
            var points = _lineSpectrum.CreateSpectrumPoints(size);

            if (points == null)
            {
                return;
            }

            currentTopIntensity = 0;
            currentTopIntensityIndex = 0;
            foreach (var p in points.Skip(1).ToArray())
            {
                if (p.Value > currentTopIntensity)
                {
                    currentTopIntensity = p.Value;
                    currentTopIntensityIndex = p.SpectrumPointIndex;
                }
            }

            if (currentTopIntensity > maxIntensity)
            {
                maxIntensity = currentTopIntensity;
            }

            currentTopIntensity = (currentTopIntensity / maxIntensity);

            if (currentTopIntensity > .70)
                UPDATE_TIMER = UPDATE_TIMER_FAST;
            else
                UPDATE_TIMER = UPDATE_TIMER_NORMAL;

            if (DateTime.Now > lastUpdatedTime.Add(TimeSpan.FromMilliseconds(UPDATE_TIMER)))
            {
                Debug.WriteLine("Loudest value is " + currentTopIntensity + " at index " + currentTopIntensityIndex);
                updateLights();
                lastUpdatedTime = DateTime.Now;
            }
        }

        private void updateLights()
        {
            var command = new LightCommand().TurnOn();

            int control = random.Next(3);
            switch (control)
            {
                case 0:
                    command.SetColor(getMain(), getSecondary(), getTertiary());
                    break;
                case 1:
                    command.SetColor(getSecondary(), getMain(), getTertiary());
                    break;
                default:
                    command.SetColor(getTertiary(), getSecondary(), getMain());
                    break;
            }

            command.Brightness = getBrightness();
            command.TransitionTime = TimeSpan.MinValue;

            _hueClient.SendCommandAsync(command, lights);
        }

        private int getMain()
        {
            int maxRed = 255;
            if (currentTopIntensity < .60)
                maxRed = 150;
            else if (currentTopIntensity < .50)
                maxRed = 100;
            else if (currentTopIntensity < .30)
                maxRed = 0;

            return (int)Math.Round(currentTopIntensity * maxRed);
        }

        private int getSecondary()
        {
            int maxGreen = 0;
            if (currentTopIntensity > .70 && currentTopIntensity < .40)
                maxGreen = 170;
            else if (currentTopIntensity < .40)
                maxGreen = 100;

            return (int)Math.Round(currentTopIntensity * maxGreen);
        }

        private int getTertiary()
        {
            int maxBlue = 127;
            if (currentTopIntensity < .60)
                maxBlue = 200;

            return (int)Math.Round(currentTopIntensity * maxBlue);
        }

        private byte getBrightness()
        {
            int minBrightness = 150;
            int maxBrightness = 254;

            if (currentTopIntensity < .70)
                maxBrightness = 230;
            else if (currentTopIntensity < .60)
                maxBrightness = 200;
            else if (currentTopIntensity < .40)
                maxBrightness = 180;
            else if (currentTopIntensity < .30)
                maxBrightness = 160;

            return (byte)((int)random.Next(minBrightness, maxBrightness));
        }
    }
}
