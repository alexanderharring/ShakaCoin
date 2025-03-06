using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSec.Cryptography;

namespace ShakaCoin.Cryptography
{
    internal class HomeKeys
    {

        private Ed25519 _algorithm = SignatureAlgorithm.Ed25519;

        private Key _key;

        internal HomeKeys()
        {
            _key = new Key(_algorithm);
        }

        public byte[] SignData(byte[] data)
        {
            return _algorithm.Sign(_key, data);
        }

        public byte[] GetPublicKey()
        {
            return _key.Export(KeyBlobFormat.RawPublicKey);
        }

        public bool VerifySignature(byte[] signature, byte[] data, byte[] pubKey)
        {

            PublicKey newPubKey = PublicKey.Import(_algorithm, pubKey, KeyBlobFormat.RawPublicKey);
            return _algorithm.Verify(newPubKey, data, signature);
        }
    }
}
