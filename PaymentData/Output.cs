using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    public class Output
    {

        public ulong Amount;

        public byte[] DestinationPublicKey = new byte[64];

        public Output(ulong amount, byte[] pubk)
        {
            Amount = amount;
            DestinationPublicKey = pubk;
        }

    }
}
