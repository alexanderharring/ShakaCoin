﻿using ShakaCoin.PaymentData;
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

        public static Transaction generateTransaction(int i)
        {
            Transaction tx = new Transaction(0x00);
            Random rnd = new Random();
            int n = rnd.Next(0, i);
            tx.AddOutput(new Output((ulong)n, Hasher.Hash256(Hasher.GetBytesQuick(n.ToString()))));
            return tx;
        }

        static void Main(string[] args)
        {
            //var m = new MainLogic();

            TXNodeAVL root = new TXNodeAVL(generateTransaction(43));

            Transaction tx42 = new Transaction(0x00);

            for (int i = 5; i < 15; i++)
            {
                Transaction tx = generateTransaction(i*43);
                if (i == 42)
                {
                    tx42 = tx;

                }

                root.Insert(tx);
            }

            root.LevelOrderTraversal();


            Console.WriteLine("");
            Console.WriteLine("");

            root.ReverseInOrder();

        }

    }
}
