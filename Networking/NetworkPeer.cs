using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;

namespace ShakaCoin.Networking
{
    internal class NetworkPeer
    {

        private TcpListener _TCPListener;
        private int _port;
        private List<TcpClient> _peers = new List<TcpClient>();
        private bool _debug;

        public NetworkPeer(bool debug)
        {

            //_port = prt;
            _port = 7770;
            _TCPListener = new TcpListener(IPAddress.Any, _port);
            _debug = debug;
        }

        public async Task StartListening()
        {
            _TCPListener.Start();

            if (_debug)
            {
                Console.WriteLine("Started listening...");
            }

            while (true)
            {
                var newClient = await _TCPListener.AcceptTcpClientAsync();
                _peers.Add(newClient);
                _ = HandleClient(newClient);

                
            }
        }

        private async Task HandleClient(TcpClient newClient)
        {
            Console.WriteLine("Initial");
            await using NetworkStream dataStream = newClient.GetStream();
            byte[] receiveBuffer = new byte[1024];

            Console.WriteLine("Client connected");

            int received = await dataStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
            string receivedHx = Convert.ToHexString(receiveBuffer);

            Console.WriteLine(receivedHx);

            newClient.Close();
        }

        public async Task SendData(IPAddress targetIpAddress, byte[] content)
        {
            TcpClient newClient = new TcpClient();
            await newClient.ConnectAsync(targetIpAddress, 7770);
            NetworkStream dataStream = newClient.GetStream();

            
            if (true)
            {
                Console.WriteLine("Sent data -> " + Convert.ToHexString(content));
            }

            await dataStream.WriteAsync(content);
        }


    }
}
