using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.SampleConverter;
using HueMusicViz.Models;
using Q42.HueApi;
using SpotifyAPI.Local;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HueMusicViz
{
    public partial class MainForm : Form
    {
        private readonly SpotifyLocalAPI _spotify;
        private readonly HueClient _hueClient;
        private readonly EchoNestClient _echoNest;

        static List<String> lights = new List<String> { "6", "7" };
        private Random random = new Random();

        private IEnumerable<Bar> bars;
        private Bar currentBar;
        private EchoNestBeat currentBeat;
        private EchoNestBeat lastBeat;

        private Boolean isOnDownbeat;
        private bool wasLastUpdateCompleted = true;

        private System.Windows.Forms.Timer hueTimer = new System.Windows.Forms.Timer();

        // #TODO: Make this not static and not hardcoded
        private static int HUE_1 = 56100;
        private static int HUE_2 = 46920;

        private int currentSaturation = -1;
        private int nextSaturation = -1;
        private int nextHue = HUE_1;
        private int nextHueIncrement = -1;

        private long lastTicks = -1;

        private static int LIGHTS_DELAY_MS = 600;

        public MainForm()
        {
            InitializeComponent();
            _spotify = new SpotifyLocalAPI();

            string bridgeIp = "192.168.1.17";
            _hueClient = new HueClient(bridgeIp);

            _echoNest = new EchoNestClient();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setup();

            hueTimer.Interval = 1;
            hueTimer.Tick += updateLights;
        }

        private async void setup()
        {
            // Set up the Spotify local API + events
            setupSpotify();
            initSpotifyStatus();

            // Set up the Hue Client
            _hueClient.Initialize("kasra-hue-music-user");

            // Setup the visualizer
            visualizer.Paint += drawVisualizer;

            // Turn the lights on so we don't have to keep sending ON in every command
            await turnLightsOn();
        }

        private void setupSpotify()
        {
            _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
            _spotify.OnTrackChange += _spotify_OnTrackChange;
            _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;
            _spotify.SynchronizingObject = this;

            _spotify.Connect();
            _spotify.ListenForEvents = true;
        }

        private void initSpotifyStatus()
        {
            var status = _spotify.GetStatus();

            if (status.Playing)
            {
                songLabel.Text = status.Track.TrackResource.Name;
                trackTimeLabel.Text = "" + status.PlayingPosition;
            }
        }

        private void _spotify_OnPlayStateChange(PlayStateEventArgs e)
        {
            if (e.Playing)
            {
                //hueTimer.Start();
            }
            else
            {
                if (hueTimer.Enabled)
                    hueTimer.Stop();
            }
        }

        private void _spotify_OnTrackChange(TrackChangeEventArgs e)
        {
            songLabel.Text = e.NewTrack.TrackResource.Name;
            getSongInfo(e.NewTrack);
        }

        private void _spotify_OnTrackTimeChange(TrackTimeChangeEventArgs e)
        {
            trackTimeLabel.Text = "" + e.TrackTime;

            if (bars == null)
                return;

            //updateCurrentBar(e.TrackTime);
            updateCurrentBar(e.TrackTime + (LIGHTS_DELAY_MS / 1000.0));

            int hue_diff = Math.Abs(HUE_1 - HUE_2);

            if (currentBar != null && currentBeat != null && currentBeat != lastBeat)
            {
                if (e.TrackTime >= currentBar.beats.First().start && e.TrackTime <= currentBar.beats.First().start + currentBar.beats.First().duration)
                {
                    isOnDownbeat = true;
                    beatLabel.Text = "Beat";
                    visualizer.Invalidate();

                    nextSaturation = 254;
                    nextHueIncrement = -1;
                    nextHue = nextHue == HUE_1 ? HUE_2 : HUE_1;

                    if (hueTimer.Enabled)
                        hueTimer.Stop();
                    hueTimer.Interval = (int)(currentBar.beats.First().duration * 1000);
                    hueTimer.Start();
                }
                else if (e.TrackTime >= currentBar.beats.ElementAt(1).start && e.TrackTime <= currentBar.beats.ElementAt(1).start + currentBar.beats.ElementAt(1).duration)
                {
                    isOnDownbeat = false;
                    beatLabel.Text = "";
                    visualizer.Invalidate();

                    nextHueIncrement = random.Next((int)(-1 * (hue_diff / 2.0)), (int)(hue_diff / 2.0));
                    nextSaturation = 50;
                }
                else if (e.TrackTime >= currentBar.beats.ElementAt(2).start && e.TrackTime <= currentBar.beats.ElementAt(2).start + currentBar.beats.ElementAt(1).duration)
                {
                    isOnDownbeat = false;
                    beatLabel.Text = "";
                    visualizer.Invalidate();

                    nextHueIncrement = random.Next((int)(-1 * (hue_diff / 2.0)), (int)(hue_diff / 2.0));
                    nextSaturation = 50;
                }
                else
                {
                    isOnDownbeat = false;
                    beatLabel.Text = "";
                    visualizer.Invalidate();

                    nextHueIncrement = random.Next((int)(-1 * (hue_diff / 2.0)), (int)(hue_diff / 2.0));
                    nextSaturation = 50;
                }
            }
        }

        private async void getSongInfo(SpotifyAPI.Local.Models.Track track)
        {
            Debug.WriteLine("New song: " + track.TrackResource.Name + "! Pausing...");
            _spotify.Pause();

            var summary = await _echoNest.getSongSummary(track.TrackResource.Uri);

            updateColorSpace(summary);
            var analysis = await _echoNest.getAnalysis(summary.analysis_url);
            bars = Bar.getBarsFromAnalysis(analysis);
        }

        private void updateColorSpace(EchoNestAudioSummary summary)
        {
            Debug.WriteLine("Danceability: " + summary.danceability + " Energy: " + summary.energy + " Valence: " + summary.valence);
            Debug.WriteLine("Resuming...");
            _spotify.Play();
        }

        private void updateCurrentBar(double currentTime)
        {
            if (currentBar != null && currentTime >= currentBar.start && currentTime <= currentBar.start + currentBar.duration)
                return;

            currentBar = null;
            foreach (var b in bars)
            {
                if (currentTime >= b.start && currentTime <= b.start + b.duration)
                {
                    currentBar = b;
                    break;
                }
            }

            if (currentBar == null)
            {
                lastBeat = currentBeat;
                currentBeat = null;
                return;
            }

            lastBeat = currentBeat;
            currentBeat = null;

            foreach (var b in currentBar.beats)
            {
                if (currentTime >= b.start && currentTime <= b.start + b.duration)
                {
                    currentBeat = b;
                    break;
                }
            }
        }

        private void drawVisualizer(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(MainForm.DefaultBackColor);

            if (isOnDownbeat)
            {
                Brush brush = new SolidBrush(Color.Red);

                var padding = (int)Math.Floor(e.ClipRectangle.Width * 0.1f);

                var width = e.ClipRectangle.Width - 2.0f * padding;
                var height = e.ClipRectangle.Height - 2.0f * padding;

                var diameter = Math.Min(width, height);

                var x = (e.ClipRectangle.Width - diameter) / 2.0f;
                var y = (e.ClipRectangle.Height - diameter) / 2.0f;

                g.FillEllipse(brush, x, y, diameter, diameter);

                brush.Dispose();
            }
        }

        private async Task<bool> turnLightsOn()
        {
            var command = new LightCommand().TurnOn();
            command.Effect = Effect.None;
            command.Hue = 56100;
            command.Brightness = 2;
            command.Saturation = 200;

            var tasks = lights.Select(i => _hueClient.SendCommandAsync(command, new List<String>() { i })).ToArray();
            await Task.WhenAll(tasks);
            return tasks.All(i => i.Result.HasErrors() == false);
        }

        private async void setSaturation(int saturation)
        {
            if (!wasLastUpdateCompleted)
            {
                Debug.WriteLine("Ignoring request to set saturation to " + saturation);
                return;
            }

            wasLastUpdateCompleted = false;
            var command = new LightCommand();
            command.TransitionTime = TimeSpan.FromSeconds(0.3);
            command.Saturation = saturation;

            var tasks = lights.Select(i => _hueClient.SendCommandAsync(command, new List<String>() { i })).ToArray();
            await Task.WhenAll(tasks);
            currentSaturation = saturation;
            wasLastUpdateCompleted = true;
        }

        private async void updateLights(object sender, EventArgs e)
        {
            if (lastTicks != -1)
            {
                Debug.WriteLine("updateLights called after " + (DateTime.UtcNow.Ticks - lastTicks) / TimeSpan.TicksPerMillisecond + "ms");
            }
            lastTicks = DateTime.UtcNow.Ticks;
            hueTimer.Stop();

            var command = new LightCommand();
            command.TransitionTime = TimeSpan.Zero;
            command.Saturation = nextSaturation;
            if (nextHueIncrement == -1)
                command.Hue = nextHue;
            else 
                command.HueIncrement = nextHueIncrement;

            await _hueClient.SendCommandAsync(command, lights);

            hueTimer.Interval = 1;
            hueTimer.Start();
        }


        private async void SendUpdate(int saturation, int hueIncrement, double transitionTime)
        {
            var command = new LightCommand();
            command.Saturation = saturation;
            command.HueIncrement = hueIncrement;
            command.TransitionTime = TimeSpan.FromSeconds(transitionTime);

            var lightTasks = lights.Select(i => _hueClient.SendCommandAsync(command, new List<String>() { i }));
            await Task.WhenAll(lightTasks);

            currentSaturation = saturation;
        }
    }
}
