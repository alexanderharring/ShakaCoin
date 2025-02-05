using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Cryptography
{
    internal class Signing
    {

        public static byte[] Sign(AsymmetricKeyParameter privKey, byte[] message)
        {
            
            DilithiumSigner signer = new DilithiumSigner();
            signer.Init(true, privKey);
            var signature = signer.GenerateSignature(message);
            return signature;
        }

        public static bool VerifySignature(AsymmetricKeyParameter pubKey, byte[] data, byte[] signature)
        {
            DilithiumSigner signer = new DilithiumSigner();
            signer.Init(false, pubKey);
            var check = signer.VerifySignature(data, signature);

            return check;
        }
    }
}
