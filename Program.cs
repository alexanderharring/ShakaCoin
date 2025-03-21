﻿using ShakaCoin.PaymentData;
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

        static async Task Main(string[] args)
        {
            var peer = new NetworkPeer(true);

            _ = peer.StartListening();

            Console.WriteLine("Looking for connection");

            Random rnd = new Random();

            List<byte> randomData = new List<byte>();

            for (int j = 0; j < (rnd.Next(100, 400)); j++)
            {
                randomData.Add((byte)rnd.Next(0, 255));
            }
            byte[] sampleData = randomData.ToArray();

            await Task.Delay(500);

            await peer.SendData(IPAddress.Parse("172.28.140.43"), sampleData);

            Console.WriteLine("here");
            Console.ReadLine();

        }
    }
}
