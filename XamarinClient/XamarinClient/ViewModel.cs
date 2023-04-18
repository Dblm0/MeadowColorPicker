using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace XamarinClient
{
    class ViewModel
    {
        Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set { _selectedColor = value; BroadCastColor(value); }
        }
        public ViewModel()
        {
            var wlanInterface = NetworkInterface.GetAllNetworkInterfaces().First(x => x.Name.Contains("wlan"));
            var ipProps = wlanInterface.GetIPProperties().UnicastAddresses
                 .First(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

            int ip = BitConverter.ToInt32(ipProps.Address.GetAddressBytes(), 0);
            int mask = BitConverter.ToInt32(ipProps.IPv4Mask.GetAddressBytes(), 0);
            _broadcastAddr = new IPAddress(BitConverter.GetBytes(ip | ~mask));
        }
        IPAddress _broadcastAddr;
        void BroadCastColor(Color color)
        {
            var bytes = Encoding.ASCII.GetBytes(color.ToHex());
            using var client = new UdpClient();
            client.EnableBroadcast = true;
            var endpoint = new IPEndPoint(_broadcastAddr, 4444);
            client.Send(bytes, bytes.Length, endpoint);
            client.Close();
        }
    }
}
