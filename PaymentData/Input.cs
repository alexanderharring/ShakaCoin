using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Cryptography;

namespace ShakaCoin.PaymentData
{
    public class Input
    {
        public byte[] TransactionID = new byte[32];

        public byte OutputIndex;

        public byte[] Signature = new byte[64];

        public Input(byte[] tx, byte outputIndex)
        {
            TransactionID = tx;
            OutputIndex = outputIndex;
        }

        public void AddSignature(byte[] sig)
        {
            Signature = sig;
        }

        public bool VerifySignature(byte[] pk)
        {
            return HomeKeys.VerifySignatureIsolated(Signature, TransactionID, pk);
        }

        public byte[] GetBytes() // 97 bytes
        {
            // TXID (32) + Oind (1) + Sig (64)

            byte[] exportBytes = new byte[97];

            Buffer.BlockCopy(TransactionID, 0, exportBytes, 0, 32);
            exportBytes[32] = OutputIndex;
            Buffer.BlockCopy(Signature, 0, exportBytes, 33, 64);

            return exportBytes;

        }


    }
}
