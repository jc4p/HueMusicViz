using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HueMusicViz
{
    public partial class SandboxForm : Form
    {
        private readonly HueClient _hueClient;
        private Timer _timer;

        static List<String> lights = new List<String> { "4", "5" };

        private int nextHue = 46920;

        private long lastTicks = -1;

        public SandboxForm()
        {
            InitializeComponent();

            string bridgeIp = "192.168.1.17";
            _hueClient = new HueClient(bridgeIp, "kasra-hue-music-user");

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1;
            _timer.Tick += updateLights;
        }

        private async void SandboxForm_Load(object sender, EventArgs e)
        {
            trackBarR.Scroll += (s, a) => { labelR.Text = "" + trackBarR.Value; };
            trackBarG.Scroll += (s, a) => { labelG.Text = "" + trackBarG.Value; };
            trackBarB.Scroll += (s, a) => { labelB.Text = "" + trackBarB.Value; };
            trackBarSaturation.Scroll += (s, a) => { labelBrightness.Text = "" + trackBarSaturation.Value; };

            await turnLightsOn();
            _timer.Start();
        }

        private async Task<bool> turnLightsOn()
        {
            var command = new LightCommand();

            command.On = true;
            command.Brightness = 254;
            command.Saturation = 254;
            command.Hue = 65280;

            var response = await _hueClient.SendCommandAsync(command, lights);
            return !response.HasErrors();
        }

        private async void updateLights(object sender, EventArgs e)
        {
            buttonUpdate.Enabled = false;
            if (lastTicks != -1)
            {
                Debug.WriteLine("updateLights called after " + (DateTime.UtcNow.Ticks - lastTicks) / TimeSpan.TicksPerMillisecond + "ms");
            }
            lastTicks = DateTime.UtcNow.Ticks;
            _timer.Stop();

            var command = new LightCommand();

            //command.SetColor(trackBarR.Value, trackBarG.Value, trackBarB.Value);
            command.TransitionTime = TimeSpan.Zero;
            command.Hue = nextHue;
            await _hueClient.SendCommandAsync(command, lights);

            if (nextHue == 46920)
                nextHue = 65280;
            else
                nextHue = 46920;

            _timer.Start();
            buttonUpdate.Enabled = true;
        }
    }
}
