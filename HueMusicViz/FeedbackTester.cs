using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HueMusicViz
{
    public partial class FeedbackTester : Form
    {
        private readonly HueClient _hueClient;

        static List<String> lights = new List<String> { "3" };

        private int nextHue = 46920;
        private long toggledPressedTicks;

        public FeedbackTester()
        {
            InitializeComponent();

            string bridgeIp = "192.168.1.17";
            _hueClient = new HueClient(bridgeIp, "kasra-hue-music-user");
        }

        private async void FeedbackTester_Load(object sender, EventArgs e)
        {
            await turnLightsOn();
        }

        private void buttonToggle_Click(object sender, EventArgs e)
        {
            buttonToggle.Enabled = false;
            Form.ActiveForm.Focus();
            toggledPressedTicks = DateTime.UtcNow.Ticks;
            updateLights();
        }

        private async Task<bool> turnLightsOn()
        {
            var command = new LightCommand();

            command.On = true;
            command.Brightness = 2;
            command.Saturation = 254;
            command.Hue = 65280;

            var response = await _hueClient.SendCommandAsync(command, lights);
            return !response.HasErrors();
        }

        private async void updateLights()
        {
            var command = new LightCommand();

            command.TransitionTime = TimeSpan.Zero;
            command.Hue = nextHue;
            await _hueClient.SendCommandAsync(command, lights);

            if (nextHue == 46920)
                nextHue = 65280;
            else
                nextHue = 46920;
        }

        private void FeedbackTester_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                textBox.Text += (DateTime.UtcNow.Ticks - toggledPressedTicks) / TimeSpan.TicksPerMillisecond + "ms\r\n";
                buttonToggle.Enabled = true;
                e.Handled = true;
            }
        }
    }
}
