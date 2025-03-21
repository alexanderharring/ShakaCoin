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
        public static readonly IPAddress BootstrapAddress = IPAddress.Parse("172.28.140.43");
        public static readonly string GetPeersCode = "AABBCC";

    }
}
