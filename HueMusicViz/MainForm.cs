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

        static List<String> lights = new List<String> { "4", "5" };
        private Random random = new Random();

        private IEnumerable<Bar> bars;
        private Bar lastBar = null;

        private static int HUE_MIN = 46920; // Blue
        private static int HUE_MAX = 65280; // Red

        // The "max "step" amount to add/subtract to HUE_2 to get HUE_1;
        private static int STEP_MAX = (HUE_MAX - HUE_MIN) / 4;
        private static int STEP_MIN = STEP_MAX / 2;

        private int HUE_1 = HUE_MIN;
        private int HUE_2 = HUE_MIN;
        
        private int lastHue = -1;

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
        }

        private async void setup()
        {
            // Set up the Spotify local API + events
            setupSpotify();
            initSpotifyStatus();

            // Set up the Hue Client
            _hueClient.Initialize("kasra-hue-music-user");

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
        }

        private void _spotify_OnTrackChange(TrackChangeEventArgs e)
        {
            songLabel.Text = e.NewTrack.TrackResource.Name;
            getSongInfo(e.NewTrack);
        }

        private async void _spotify_OnTrackTimeChange(TrackTimeChangeEventArgs e)
        {
            trackTimeLabel.Text = "" + e.TrackTime;

            if (bars == null)
                return;

            var timeUpdatedForLightDelay = e.TrackTime + (LIGHTS_DELAY_MS / 1000.0);
            Bar currentBar = getCurrentBar(timeUpdatedForLightDelay);
            if (currentBar != null)
                await updateLights(currentBar, timeUpdatedForLightDelay);
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

            // Okay here's where it gets fun.
            // The higher the energy is, the redder we want the color to be, with energy of 1.0 == HUE_MAX (brightest red).
            // Then, we want to generate a second color that's within the same range, by moving an amount in either direction that's within
            // the bounds of (STEP_MIN, STEP_MAX). However, if we go _over_ HUE_MAX or _under_ HUE_MIN, let's flip the direction of the step.
            HUE_1 = HUE_MIN + (int)(((HUE_MAX - HUE_MIN) * summary.energy));
            int stepAmount = random.Next(STEP_MIN, STEP_MAX);
            stepAmount *= random.Next(2) == 1 ? 1 : -1;

            HUE_2 = HUE_1 + stepAmount;
            if (HUE_2 > HUE_MAX)
            {
                HUE_2 = HUE_1 - stepAmount;
            }
            else if (HUE_2 < HUE_MIN)
            {
                HUE_2 = HUE_1 - stepAmount;
            }

            lastHue = HUE_1;

            Debug.WriteLine("Setting base hues to " + HUE_1 + " and " + HUE_2);

            Debug.WriteLine("Resuming...");
            _spotify.Play();
        }

        private Bar getCurrentBar(double currentTime)
        {
            // Don't bother looking for the bar if it's the same one as last time
            if (lastBar != null && currentTime >= lastBar.start && currentTime <= (lastBar.start + lastBar.duration))
                return null;

            Bar currentBar = null;
            foreach (var b in bars)
            {
                if (currentTime >= b.start && currentTime <= b.start + b.duration)
                {
                    currentBar = b;
                    break;
                }
            }

            if (currentBar != null)
                lastBar = currentBar;

            return currentBar;
        }

        private async Task<bool> updateLights(Bar currentBar, double trackTime)
        {
            Debug.WriteLine("Got a new bar: " + currentBar + ", we're at time: " + trackTime);
            // If we're in the first beat in the measure...
            if (trackTime >= currentBar.beats.First().start && trackTime <= currentBar.beats.First().start + currentBar.beats.First().duration)
            {
                int downBeatHue = lastHue == HUE_1 ? HUE_2 : HUE_1;
                int lullHue = lastHue;
                lastHue = downBeatHue;

                // Pop to color on the down-beat
                await SendUpdate(254, downBeatHue, 0);

                double timeLeftInBar = currentBar.duration - (trackTime - currentBar.start);
                // Setting saturation low so they lull as they get farther from the down-beat.
                return await SendUpdate(127, lullHue, timeLeftInBar);
            }

            return false;
        }

        private async Task<bool> turnLightsOn()
        {
            var command = new LightCommand().TurnOn();
            command.Effect = Effect.None;
            command.Hue = HUE_1;
            command.Brightness = 127;
            command.Saturation = 254;

            var result = await _hueClient.SendCommandAsync(command, lights);

            return result.All(i => i.Error == null);
        }

        private async Task<bool> SendUpdate(int saturation, int hue, double transitionTime)
        {
            var command = new LightCommand();
            command.Saturation = saturation;
            command.Hue = hue;
            command.TransitionTime = TimeSpan.FromSeconds(transitionTime);

            var result = await _hueClient.SendCommandAsync(command, lights);

            return result.All(i => i.Error == null);
        }
    }
}
