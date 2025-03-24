using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Networking
{
    internal class NetworkConstants
    {
        public static readonly int Port = 7770; 
        public static readonly IPAddress BootstrapAddress = IPAddress.Parse("172.22.86.12");
        public static readonly string GetPeersCode = "AABBCC";
        public static readonly string PingCode = "AACCBB";
        public static readonly string PongCode = "CCAABB";
        public static readonly string TransactionCode = "ABCABC";
        public static readonly string BlockCode = "CABCAB";
        public static readonly string RequestBlock = "CAABBC";
        public static readonly string RequestMerkleProof = "BAACCB";
        public static readonly int PingDuration = 3000;
        public static readonly int DiffusionNumber = 3;
        

    }
}
