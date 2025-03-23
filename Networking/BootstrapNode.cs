using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

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

            Console.WriteLine("Started bootstrap node");

            while (true)
            {
                
                var newClient = await _listener.AcceptTcpClientAsync();

                var newPeer = new Peer(newClient);

                _peers.Add(newPeer);

                Console.WriteLine("Connected to new peer @ " + newPeer.GetIP());

                _ = HandleNewPeer(newPeer);
            }
        }

        private async Task HandleNewPeer(Peer peer)
        {
            byte[] msg = peer.ReceiveMessage().Result;

            if (Hasher.GetHexStringQuick(msg) == NetworkConstants.GetPeersCode)
            {

                List<string> ips = new List<string>();


                foreach (Peer p in _peers)
                {
                    ips.Add(p.GetIP());
                }

                string buildPeers = string.Join(",", ips);

                await peer.SendMessage(Hasher.GetBytesQuick(buildPeers));

                Console.WriteLine("Sent list of nodes to " + peer.GetIP());

            } else
            {
                Console.WriteLine("idk whats going on");
            }
        }

    }
}
