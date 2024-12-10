using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.BC;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Microsoft.VisualBasic;


namespace ShakaCoin.Cryptography
{
    public class MainCryptography
    {

        internal HomeKeys mainKeys;

        public MainCryptography()
        {
            mainKeys = new HomeKeys();


        }

        internal byte[] SignSignature(AsymmetricKeyParameter privKey, string message)
        {
            return Signing.Sign(privKey, message);
        }

        internal bool VerifySignature(AsymmetricKeyParameter pubKey, string message, byte[] signature)
        {
            return Signing.VerifySignature(pubKey, message, signature);
        }

        public string GetPublicKey()
        {

            return mainKeys.GetPublic();
        }

        public string GetPrivateKey()
        {
            return mainKeys.GetPrivate();
        }

    }
}
