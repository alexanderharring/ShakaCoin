using ShakaCoin.PaymentData;
using System.Text;
using ShakaCoin.Networking;
using System.Collections;
using System.Net;
using ShakaCoin.Blockchain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using ShakaCoin.Datastructures;
using System.Runtime.InteropServices;

namespace ShakaCoin
{
    public class Program
    {
        //static async Task Main(string[] args)
        //{
        //    NetworkPeer nPeer = new NetworkPeer(true);

        //    _ = nPeer.StartListening();

        //    await Task.Delay(2500);


        //    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        //    await nPeer.SendData(host.AddressList[0], Convert.FromHexString("abcdef"));

        //}

        private static Transaction generateTransaction()
        {
            Transaction tx = new Transaction(0x00);
            Random rnd = new Random();
            int n = rnd.Next(0, int.MaxValue / 2);

            Input ix = new Input(Hasher.Hash256(Hasher.GetBytesQuick((n + 1).ToString())), 8);
            ix.AddSignature(Hasher.Hash512(Hasher.GetBytesQuick((n + 2).ToString())));

            tx.AddInput(ix);
            tx.AddOutput(new Output((ulong)n, Hasher.Hash256(Hasher.GetBytesQuick(n.ToString()))));
            return tx;
        }

        public static async Task RunBootstrap()
        {
            var bootstrap = new BootstrapNode();
            await bootstrap.Start();
        }

        public static async Task RunPeerNode()
        {
            var peerManager = new PeerManager();
            await peerManager.ConnectToBootstrapNode();
            await peerManager.Start();

        }

        static async Task Main(string[] args)
        {
            await RunPeerNode();


        }
    }
}
