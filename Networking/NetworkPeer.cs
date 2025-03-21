using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using ShakaCoin.Blockchain;

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
            Console.WriteLine("Client connected...");

            NetworkStream dataStream = newClient.GetStream();

            List<byte> dataList = new List<byte>();

            byte[] receiveBuffer = new byte[4096];

            int received = 0;


            while ((received = await dataStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length)) > 0)
            {
                Console.WriteLine(received);
                dataList.AddRange(receiveBuffer.Take(received));

            }
            
            string bigMessage = Hasher.GetStringQuick(dataList.ToArray());

            Console.WriteLine("Received... " + bigMessage);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            newClient.Close();
        }

        public async Task SendData(IPAddress targetIpAddress, byte[] content)
        {
            TcpClient newClient = new TcpClient();
            await newClient.ConnectAsync(targetIpAddress, 7770);
            NetworkStream dataStream = newClient.GetStream();

            
            if (_debug)
            {
                Console.WriteLine("Sent data -> " + Hasher.GetHexStringQuick(content));
                Console.WriteLine("");
            }

            await dataStream.WriteAsync(content);
        }


    }
}
