using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Networking
{
    internal class BootstrapNode
    {
        private TcpListener _listener;
        private List<Peer> _peers = new List<Peer>();

        public BootstrapNode()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
            
        }

        public async Task Start()
        {
            _listener.Start();

            while (true)
            {
                var newClient = await _listener.AcceptTcpClientAsync();
                var newPeer = new Peer(newClient);

                _peers.Add(newPeer);

                Console.WriteLine("Connected to new peer ");

                _ = Handlecli
            }
        }

    }
}
