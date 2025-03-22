using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

namespace ShakaCoin.Networking
{
    internal class P2PServer
    {

        private TcpListener _listener;
        private bool _running;

        public P2PServer()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
        }

        public async Task Start()
        {
            _listener.Start();
            _running = true;

            Console.WriteLine("Started running P2P server on " + NetworkConstants.Port.ToString());

            while (_running)
            {
                var newClient = await _listener.AcceptTcpClientAsync();

                var newPeer = new Peer(newClient);

                Console.WriteLine("Connected to new peer");

                _ = HandlePeer(newPeer);
            }
        }

        private async Task HandlePeer(Peer peer)
        {
            byte[] msg = await peer.ReceiveMessage();

            string hexCode = Hasher.GetHexStringQuick(msg);

            if (hexCode == NetworkConstants.GetPeersCode)
            {
                await peer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.GetPeersCode));
            }
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
        }

        
    }
}
