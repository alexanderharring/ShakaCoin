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
        public byte[] TransactionID = new byte[64];

        public bool IsReturner;

        public byte[] Signature = new byte[64];

        public Input(byte[] tx, bool returner)
        {
            TransactionID = tx;
            IsReturner = returner;
        }

        public void AddSignature()

        //public void AddSignature(AsymmetricKeyParameter privKey)
        //{
        //    Signature = Signing.Sign(privKey, TransactionID);
        //}

        //public bool VerifyInputSignature(AsymmetricKeyParameter pubKey)
        //{
        //    return Signing.VerifySignature(pubKey, TransactionID, Signature);
        //}

    }
}
