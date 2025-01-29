using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class Output
    {

        public ulong Amount;

        public byte[] Destination = new byte[64];

    }
}
