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
            await _stream.WriteAsync(msg, 0, msg.Length);
        }

        public async Task<byte[]> ReceiveMessage()
        {
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
                ipAddress = ipAddress.MapToIPv4();
            }

            return ipEndP.Address.ToString();
        }

        public string GetMyIP()
        {
            IPEndPoint ipEndP = (IPEndPoint)_client.Client.LocalEndPoint;
            IPAddress ipAddress = ipEndP.Address;

            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            return ipEndP.Address.ToString();
        }

        public void Close()
        {
            _stream?.Close();
            _client.Close();
        }
    }
}
