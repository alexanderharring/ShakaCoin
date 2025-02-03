using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class Input
    {
        public byte[] TransactionID = new byte[64];

        public bool IsReturner;

        public byte[] Signature = new byte[64];

        internal Input(byte[] tx, bool returner, byte[] signature)
        {
            TransactionID = tx;
            IsReturner = returner;
            Signature = signature;
        }

    }
}
