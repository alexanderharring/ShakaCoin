using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Networking
{
    internal class Peer
    {
        private TcpClient _client;
        private NetworkStream _stream => _client.GetStream();

        public Peer(TcpClient client)
        {
            _client = client;
        }

        public async Task SendMessage(byte[] msg)
        {

            Task t = _stream.WriteAsync(msg, 0, msg.Length);
            
        }

        public async Task<byte[]> ReceiveMessage()
        {
            if (!_client.Connected)
            {
                Close();
                return new byte[1];
            }

            byte[] buffer = new byte[4096];

            int bRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

            return buffer.Take(bRead).ToArray();
        }

        public string GetIP()
        {
            IPEndPoint ipEndP = (IPEndPoint)_client.Client.RemoteEndPoint;
            IPAddress ipAddress = ipEndP.Address;

            if (ipAddress.IsIPv4MappedToIPv6)
            {
                return ipAddress.MapToIPv4().ToString();
            }

            return ipAddress.ToString();
        }

        public string GetMyIP()
        {
            IPEndPoint ipEndP = (IPEndPoint)_client.Client.LocalEndPoint;
            IPAddress ipAddress = ipEndP.Address;

            if (ipAddress.IsIPv4MappedToIPv6)
            {
                return ipAddress.MapToIPv4().ToString();
            }

            return ipAddress.ToString();
        }

        public void Close()
        {
            _client.Close();
        }
    }
}
