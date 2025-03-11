using ShakaCoin.PaymentData;
using System.Text;
using ShakaCoin.Networking;
using System.Collections;
using System.Net;
using ShakaCoin.Blockchain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        static void Main(string[] args)
        {
            //MainLogic.Initiate();


            FileManagement fm = new FileManagement();

            Console.WriteLine(fm.ReadWallet("exampleWallet").Length);

        }

    }
}
