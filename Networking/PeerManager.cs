using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.PaymentData;
using System.Collections.Concurrent;

namespace ShakaCoin.Networking
{
    internal class PeerManager
    {
        private ConcurrentDictionary<string, Peer> _peerDict = new ConcurrentDictionary<string, Peer>();
        private TcpListener _listener;
        private Peer? _bootstrapNode;
        private bool _amIBootstrap = false;
        private bool _running;
        public BlockchainHandler? _blockchainHandler;

        public PeerManager()
        {
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
        }

        public PeerManager(bool amBoot)
        {
            if (amBoot)
            {
                _amIBootstrap = true;
            }
            _listener = new TcpListener(IPAddress.Any, NetworkConstants.Port);
        }

        public async Task Start()
        {
            _listener.Start();
            _running = true;

            Console.WriteLine("Started running P2P server on " + NetworkConstants.Port.ToString());
            if (_amIBootstrap)
            {
                Console.WriteLine("This node is a bootstrap node.");
            }
            _ = CheckPeerStatuses();

            while (_running)
            {

                var newClient = await _listener.AcceptTcpClientAsync();

                var newPeer = new Peer(newClient);

                if (_peerDict.Count > 64)
                {
                    newPeer.Close();
                    continue;
                }


                Console.WriteLine("Accepted connection to new peer @ " + newPeer.GetIP());

                _peerDict[newPeer.GetIP()] = newPeer;

                _ = HandlePeer(newPeer);
            }
        }

        private async Task HandlePeer(Peer peer)
        {

            try
            {
                while (peer.IsConnected())
                {
                    byte[] receivedData = await peer.ReceiveMessage();

                    if (receivedData == null)
                    {
                        Console.WriteLine("Received null data, client disconnected maybe?");
                    } else
                    {
                        byte[] first3Bytes = new byte[3];
                        Buffer.BlockCopy(receivedData, 0, first3Bytes, 0, 3);

                        string hexCode = Hasher.GetHexStringQuick(first3Bytes);

                        byte[] otherData = new byte[0];

                        if (receivedData.Length > 3)
                        {
                            otherData = new byte[receivedData.Length - 3];
                            Buffer.BlockCopy(receivedData, 3, otherData, 0, receivedData.Length - 3);
                        }

                        if (hexCode == NetworkConstants.GetPeersCode)
                        {

                            byte[] buildPeers = Hasher.GetBytesQuick(string.Join(",", _peerDict.Keys.ToArray()));

                            byte[] bigArr = new byte[buildPeers.Length + 3];
                            Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.GotPeersCode), 0, bigArr, 0, 3);
                            Buffer.BlockCopy(buildPeers, 0, bigArr, 3, buildPeers.Length);

                            await peer.SendMessage(bigArr);

                            Console.WriteLine("Sent list of nodes to " + peer.GetIP());

                        }

                        else if (hexCode == NetworkConstants.GotPeersCode)
                        {

                            string PeerList = Hasher.GetStringQuick(otherData);

                            string[] ipAds = PeerList.Split(",");

                            string myIP = peer.GetMyIP();

                            DisplayPeerList(ipAds, myIP);

                            foreach (string ip in ipAds)
                            {
                                if (ip != myIP)
                                {
                                    await ConnectToNewPeer(ip);
                                }

                            }
                        }

                        else if (hexCode == NetworkConstants.PingCode)
                        {

                            Console.WriteLine("Received ping from " + peer.GetIP());
                            await peer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.PongCode));

                        }
                        else if (hexCode == NetworkConstants.PongCode)
                        {
                            peer.SetPonged();

                        }
                        else if (hexCode == NetworkConstants.TransactionCode)
                        {
                            byte[] tx = new byte[otherData.Length - 1];
                            Buffer.BlockCopy(otherData, 0, tx, 0, otherData.Length - 1);

                            _blockchainHandler.LoadTransaction(tx, otherData[otherData.Length - 1]);

                        }
                        else if (hexCode == NetworkConstants.BlockCode)
                        {
                            byte[] bk = new byte[otherData.Length - 1];
                            Buffer.BlockCopy(otherData, 0, bk, 0, otherData.Length - 1);

                            _blockchainHandler.LoadBlock(bk, otherData[otherData.Length - 1]);

                        }
                        else if (hexCode == NetworkConstants.RequestMaxBlockHeight)
                        {

                            byte[] heightDat = BitConverter.GetBytes(FileManagement.Instance.maxBlockNum);
                            byte[] bigArr = new byte[7];

                            Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.ReturnMaxBlockHeight), 0, bigArr, 0, 3);
                            Buffer.BlockCopy(heightDat, 0, bigArr, 3, 4);

                            await peer.SendMessage(bigArr);
                        }
                        else if (hexCode == NetworkConstants.ReturnMaxBlockHeight)
                        {
                            uint maxH = BitConverter.ToUInt32(otherData);

                            for (uint blockCount=(FileManagement.Instance.maxBlockNum)+1; blockCount <= maxH; blockCount++)
                            {
                                byte[] heightDat = BitConverter.GetBytes(blockCount);
                                byte[] bigArr = new byte[7];

                                Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.RequestBlock), 0, bigArr, 0, 3);
                                Buffer.BlockCopy(heightDat, 0, bigArr, 3, 4);

                                await peer.SendMessage(bigArr);
                            }
                        }
                        else if (hexCode == NetworkConstants.ReturnMaxBlockHeight)
                        {
                            uint maxH = BitConverter.ToUInt32(otherData);

                            byte[] dataM = FileManagement.ReadBlock(maxH);

                            byte[] bigArr = new byte[dataM.Length + 3];
                            Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.ReturnBlock), 0, bigArr, 0, 3);
                            Buffer.BlockCopy(dataM, 0, bigArr, 3, dataM.Length);

                            await peer.SendMessage(bigArr);
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Error here: " + ex.Message);
            } 
            finally
            {
                KillPeer(peer);
                Console.WriteLine("Closed connection to " + peer.GetIP());
            }

        }

        private async Task CheckPeerStatuses()
        {
            while (true)
            {
                foreach (Peer checkPeer in _peerDict.Values)
                {
                    _ = CheckThisPeerStatus(checkPeer);
                }

                await Task.Delay(NetworkConstants.PingDuration);
            }
        }

        private async Task CheckThisPeerStatus(Peer checkPeer)
        {
            await checkPeer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.PingCode));
            checkPeer.SetPinged();
            Console.WriteLine("Sending ping to " + checkPeer.GetIP());

            await Task.Delay(NetworkConstants.AcceptableWaitPing);

            if (!checkPeer.IsDeltaPongAcceptable())
            {
                Console.WriteLine("Peer @ " + checkPeer.GetIP() + " failed ping test");
                KillPeer(checkPeer);
            } else
            {
                Console.WriteLine("Peer @ " + checkPeer.GetIP() + " passed the ping test");
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

            _peerDict[newPeer.GetIP()] = newPeer;

            _ = HandlePeer(newPeer);

            Console.WriteLine("Connected to bootstrap node.");
            Console.WriteLine("This node's IP is " + newPeer.GetMyIP());

            await newPeer.SendMessage(Hasher.GetBytesFromHexStringQuick(NetworkConstants.GetPeersCode));
        }

        public async Task DiffuseTransaction(Transaction tx, byte hopCount)
        {
            Console.WriteLine("diffusing tx");

            //byte[] txBytes = tx.GetBytes();

            //byte[] message = new byte[txBytes.Length + 4];
            //Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.TransactionCode), 0, message, 0, 3);
            //Buffer.BlockCopy(txBytes, 0, message, 3, txBytes.Length);
            //message[txBytes.Length + 3] = hopCount;

            //Peer[] targets = GetNPeers(NetworkConstants.DiffusionNumber);

            //foreach (Peer t in targets)
            //{
            //    await t.SendMessage(message);
            //}
        }

        public async Task DiffuseBlock(Block blk, byte hopCount)
        {
            Console.WriteLine("diffusing blk");

            //byte[] blkBytes = blk.GetBlockBytes();

            //byte[] message = new byte[blkBytes.Length + 4];
            //Buffer.BlockCopy(Hasher.GetBytesFromHexStringQuick(NetworkConstants.BlockCode), 0, message, 0, 3);
            //Buffer.BlockCopy(blkBytes, 0, message, 3, blkBytes.Length);
            //message[blkBytes.Length + 3] = hopCount;

            //Peer[] targets = GetNPeers(NetworkConstants.DiffusionNumber);

            //foreach (Peer t in targets)
            //{
            //    await t.SendMessage(message);
            //}
        }

        public async Task ConnectToNewPeer(string ipAd)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(IPAddress.Parse(ipAd), NetworkConstants.Port);

            var newPeer = new Peer(tcpClient);
            _peerDict[newPeer.GetIP()] = newPeer;

            Console.WriteLine("Connected to other peer @ " + newPeer.GetIP());
        }

        public List<Peer> ListPeers()
        {
            return _peerDict.Values.ToList();
        }

        public Peer[] GetNPeers(int n)
        {
            
            if (n >= _peerDict.Count)
            {
                return _peerDict.Values.ToArray();
            } else
            {
                HashSet<Peer> getPeers = new HashSet<Peer>();
                Peer[] prs = _peerDict.Values.ToArray();
                Random rnd = new Random();

                while (getPeers.Count < n)
                {
                    getPeers.Add(prs[rnd.Next(prs.Length)]);
                }

                return getPeers.ToArray();
            }
        }

        private void KillPeer(Peer peer)
        {
            if (!_peerDict.TryRemove(peer.GetIP(), out peer))
            {
                Console.WriteLine("Failed to kill peer from dict");
                peer.Close();
            }
        }

    }
}
