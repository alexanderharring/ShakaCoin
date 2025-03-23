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

        public PeerManager()
        {

        }

        private void DisplayPeerList(string[] peerList, string myip)
        {

            Console.WriteLine("Listing " + peerList.Length.ToString() + " peers");
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
        }

        public async Task ConnectToBootstrapNode()
        {
            var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(NetworkConstants.BootstrapAddress, NetworkConstants.Port);

            var newPeer = new Peer(tcpClient);
            _peers.Add(newPeer);

            Console.WriteLine("Connected to bootstrap node.");

            await newPeer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.GetPeersCode));

            byte[] returnedPeerList = await newPeer.ReceiveMessage();

            string PeerList = Hasher.GetStringQuick(returnedPeerList);

            string[] ipAds = PeerList.Split(",");

            string myIP = newPeer.GetMyIP();

            Console.WriteLine(myIP);

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

            Console.WriteLine("Connected to other peer ");
        }

        public List<Peer> ListPeers()
        {
            return _peers;
        }

    }
}
