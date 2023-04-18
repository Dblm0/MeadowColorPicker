using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace MeadowColorPicker
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed _onboardLed;
        const int UDP_PORT = 4444;
        public MeadowApp()
        {
            Initialize();
            if (WifiConnect())
            {
                ListenUDP();
            }
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            _onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
        }
        bool WifiConnect()
        {
            _onboardLed.StartPulse(Color.Yellow);
            var connectionResult = Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD)
                .GetAwaiter().GetResult().ConnectionStatus;
            bool connected = connectionResult == Meadow.Gateway.WiFi.ConnectionStatus.Success;
            _onboardLed.Stop();
            _onboardLed.SetColor(connected ? Color.Green : Color.Red);
            return connected;
        }
        void ListenUDP()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, UDP_PORT);
            UdpClient client = new UdpClient(UDP_PORT);
            while (true)
            {
                var result = client.Receive(ref endpoint);
                var message = Encoding.ASCII.GetString(result);
                //Console.WriteLine(message);
                _onboardLed.SetColor(Color.FromHex(message));
                Thread.Sleep(0);
            }
        }
    }
}
