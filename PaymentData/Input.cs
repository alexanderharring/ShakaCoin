using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Input
    {
        public byte[] TransactionID = new byte[32];

        public byte OutputIndex;

        public byte[] Signature = new byte[64];

        public byte[] Outpoint = new byte[33];

        public Input(byte[] tx, byte outputIndex)
        {
            TransactionID = tx;
            OutputIndex = outputIndex;

            Buffer.BlockCopy(TransactionID, 0, Outpoint, 0, 32);
            Outpoint[32] = OutputIndex;
        }

        public void AddSignature(byte[] sig)
        {
            Signature = sig;
        }

        public bool VerifySignature(byte[] pk)
        {
            return Wallet.VerifySignature(Signature, Outpoint, pk);
        }

        public bool IsCoinbase()
        {
            return ((TransactionID == new byte[32]) && (OutputIndex == 255));
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
