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
        IPAddress GetBroadcastAddress(UnicastIPAddressInformation source)
        {
            int ip = BitConverter.ToInt32(source.Address.GetAddressBytes(), 0);
            int mask = BitConverter.ToInt32(source.IPv4Mask.GetAddressBytes(), 0);
            return new IPAddress(BitConverter.GetBytes(ip | ~mask));
        }

        UnicastIPAddressInformation GetIpv4Address(NetworkInterface nwInterface)
            => nwInterface.GetIPProperties().UnicastAddresses
            .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

        bool HasUnicastIpv4(NetworkInterface nwInterface)
          => nwInterface.GetIPProperties().UnicastAddresses
            .Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork);


        public List<(string name, IPAddress broadcastAddress)> NetworkNames => NetworkInterface.GetAllNetworkInterfaces()
            .Where(HasUnicastIpv4)
            .Select(x => (name: x.Name, broadcastAddress: GetBroadcastAddress(GetIpv4Address(x))))
            .ToList();

        public (string name, IPAddress broadcastAddress) SelectedNwName { get; set; }

        void BroadCastColor(Color color)
        {
            if (SelectedNwName.broadcastAddress == null) return;

            var bytes = Encoding.ASCII.GetBytes(color.ToHex());
            using var client = new UdpClient();
            client.EnableBroadcast = true;
            var endpoint = new IPEndPoint(SelectedNwName.broadcastAddress, 4444);
            client.Send(bytes, bytes.Length, endpoint);
            client.Close();
        }
    }
}
