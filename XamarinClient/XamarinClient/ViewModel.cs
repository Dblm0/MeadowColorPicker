using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

        void BroadCastColor(Color color)
        {
            var bytes = Encoding.ASCII.GetBytes(color.ToHex());
            using var client = new UdpClient();
            client.EnableBroadcast = true;
            var endpoint = new IPEndPoint(IPAddress.Parse("192.168.43.255"), 4444);
            client.Send(bytes, bytes.Length, endpoint);
            client.Close();
        }
    }
}
