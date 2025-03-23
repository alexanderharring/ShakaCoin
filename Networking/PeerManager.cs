using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Networking
{
    internal class PeerManager
    {
        private HashSet<Peer> _peers = new HashSet<Peer>();
        private TcpListener _listener;
        private Peer? _bootstrapNode;
        private bool _running;

        public PeerManager()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
        }

        public async Task Start()
        {
            _listener.Start();
            _running = true;

            Console.WriteLine("Started running P2P server on " + NetworkConstants.Port.ToString());

            _ = CheckPeerStatuses();

            while (_running)
            {
                var newClient = await _listener.AcceptTcpClientAsync();

                var newPeer = new Peer(newClient);

                if (_peers.Count > 64)
                {
                    newPeer.Close();
                    continue;
                }

                _peers.Add(newPeer);

                Console.WriteLine("Accepted connection to new peer @ " + newPeer.GetIP());

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
            else if (hexCode == NetworkConstants.PingCode)
            {
                await peer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.PongCode));
            }
        }

        private async Task CheckPeerStatuses()
        {
            while (true)
            {
                foreach (Peer checkPeer in _peers)
                {
                    await CheckThisPeerStatus(checkPeer);
                }

                await Task.Delay(NetworkConstants.PingDuration);
            }
        }

        private async Task CheckThisPeerStatus(Peer checkPeer)
        {
            await checkPeer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.PingCode));
            var res = await checkPeer.ReceiveMessage();

            if (Hasher.GetHexStringQuick(res) != NetworkConstants.PongCode)
            {
                Console.WriteLine("Peer @ " + checkPeer.GetIP() + " failed ping test");
                checkPeer.Close();
                _peers.Remove(checkPeer);
            }
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
        }

        private void DisplayPeerList(string[] peerList, string myip)
        {

            Console.WriteLine("Listing " + peerList.Length.ToString() + " peers");
            Console.WriteLine("");
            for (int i = 0; i < peerList.Length; i++)
            {
                string ipN = peerList[i];

                if (ipN == myip)
                {
                    Console.WriteLine("#" + i.ToString() + " - " + ipN + " ( you )");
                } else
                {
                    Console.WriteLine("#" + i.ToString() + " - " + ipN);
                }
            }
            Console.WriteLine("");
        }

        public async Task ConnectToBootstrapNode()
        {
            var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(NetworkConstants.BootstrapAddress, NetworkConstants.Port);

            var newPeer = new Peer(tcpClient);

            _bootstrapNode = newPeer;

            Console.WriteLine("Connected to bootstrap node.");
            Console.WriteLine("This node's IP is " + newPeer.GetMyIP());

            await newPeer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.GetPeersCode));

            byte[] returnedPeerList = await newPeer.ReceiveMessage();

            string PeerList = Hasher.GetStringQuick(returnedPeerList);

            string[] ipAds = PeerList.Split(",");

            string myIP = newPeer.GetMyIP();


            DisplayPeerList(ipAds, myIP);

            foreach (string ip in ipAds)
            {
                if (ip != myIP)
                {
                    await ConnectToNewPeer(ip);
                }
                
            }
        }

        public async Task ConnectToNewPeer(string ipAd)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(IPAddress.Parse(ipAd), NetworkConstants.Port);

            var newPeer = new Peer(tcpClient);
            _peers.Add(newPeer);

            Console.WriteLine("Connected to other peer @ " + newPeer.GetIP());
        }

        public List<Peer> ListPeers()
        {
            return _peers.ToList();
        }

        public Peer[] GetNPeers(int n)
        {
            
            if (n >= _peers.Count)
            {
                return _peers.ToArray();
            } else
            {
                HashSet<Peer> getPeers = new HashSet<Peer>();
                Peer[] prs = _peers.ToArray();
                Random rnd = new Random();

                while (getPeers.Count < n)
                {
                    getPeers.Add(prs[rnd.Next(prs.Length)]);
                }

                return getPeers.ToArray();
            }
        }

    }
}
