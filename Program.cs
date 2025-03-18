using ShakaCoin.PaymentData;
using System.Text;
using ShakaCoin.Networking;
using System.Collections;
using System.Net;
using ShakaCoin.Blockchain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using ShakaCoin.Datastructures;

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
            tx.AddOutput(new Output((ulong)n, Hasher.Hash256(Hasher.GetBytesQuick(n.ToString()))));
            return tx;
        }

        static void Main(string[] args)
        {

            Transaction newTx = generateTransaction();

            Transaction recon = Parser.ParseTransaction(newTx.GetBytes());

            Console.WriteLine(recon.GetBytes().Length);

            //Assert.AreEqual(Hasher.GetHexStringQuick(newTx.GetBytes()), Hasher.GetHexStringQuick(recon.GetBytes()));

        }

    }
}
