using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Networking
{
    internal class PeerManager
    {
        private List<Peer> _peers = new List<Peer>();
        private TcpListener _listener;
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

            while (_running)
            {
                var newClient = await _listener.AcceptTcpClientAsync();

                var newPeer = new Peer(newClient);

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
            _peers.Add(newPeer);

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
            return _peers;
        }

    }
}
