using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;

namespace MeadowColorPicker
{
    public class MeadowApp : App<F7FeatherV1>
    {
        RgbPwmLed _onboardLed;
        const int UDP_PORT = 4444;

        public override async Task Run()
        {
            _onboardLed.StartPulse(Color.Yellow);
            var connected = await WifiConnect();
            _onboardLed.Stop();
            _onboardLed.SetColor(connected ? Color.Green : Color.Red);

            if (connected)
            {
                ListenUDP();
            }

            await base.Run();
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            _onboardLed = new RgbPwmLed(
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);
            return base.Run();
        }

        async Task<bool> WifiConnect()
        {
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            try
            {
                await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void ListenUDP()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, UDP_PORT);
            UdpClient client = new UdpClient(UDP_PORT);
            while (true)
            {
                var result = client.Receive(ref endpoint);
                var message = Encoding.ASCII.GetString(result);
                _onboardLed.SetColor(Color.FromHex(message));
                Thread.Sleep(0);
            }
        }
    }
}
