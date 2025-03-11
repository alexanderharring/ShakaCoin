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

        public byte[] DestinationPublicKey = new byte[32];

        public Output(ulong amount, byte[] pubk)
        {
            Amount = amount;
            DestinationPublicKey = pubk;
        }


        public byte[] ExportToBytes() //40 bytes
        {
            byte[] output = new byte[40];

            Buffer.BlockCopy(DestinationPublicKey, 0, output, 0, 32);

            Buffer.BlockCopy(BitConverter.GetBytes(Amount), 0, output, 32, 8);

            return output;
        }

    }
}
