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

            foreach (string ip in ipAds)
            {
                await ConnectToNewPeer(ip);
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
