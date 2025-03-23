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
        private HashSet<Peer> _peers = new HashSet<Peer>();

        public BootstrapNode()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
            
        }

        public async Task Start()
        {
            _listener.Start();

            _ = CheckPeerStatuses();

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
            }
            else if (Hasher.GetHexStringQuick(msg) == NetworkConstants.PingCode)
            {
                await peer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.PongCode));
            }
            else
            {
                Console.WriteLine("Non get-peers request asked to bootsrap node :(");
            }
        }

        private async Task CheckPeerStatuses()
        {
            while (true)
            {
                Console.WriteLine("hre");

                foreach (Peer checkPeer in _peers)
                {
                    await CheckThisPeerStatus(checkPeer);
                }

                await Task.Delay(NetworkConstants.PingDuration);
                Console.WriteLine("Hree");
            }
        }

        private async Task CheckThisPeerStatus(Peer checkPeer)
        {
            Console.WriteLine("A");
            await checkPeer.SendMessage(Hasher.GetBytesQuick(NetworkConstants.PingCode));
            Console.WriteLine("B");
            var res = await checkPeer.ReceiveMessage();
            Console.WriteLine("C");

            if (Hasher.GetHexStringQuick(res) != NetworkConstants.PongCode)
            {
                Console.WriteLine("Peer @ " + checkPeer.GetIP() + " failed ping test");
                checkPeer.Close();
                _peers.Remove(checkPeer);
            }
        }

    }
}
