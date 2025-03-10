using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSec.Cryptography;
namespace ShakaCoin.Cryptography
{
    public class HomeKeys
    {

        private Ed25519 _algorithm = SignatureAlgorithm.Ed25519;

        private Key _key;



        public HomeKeys()
        {
            _key = new Key(_algorithm);
        }

        public HomeKeys(byte[] privkey)
        {
            _key = Key.Import(_algorithm, privkey, KeyBlobFormat.RawPrivateKey);
        }

        public byte[] SignData(byte[] data)
        {
            return _algorithm.Sign(_key, data);
        }

        //public byte[] GetPrivateKey()
        //{
        //    return _key.Export(KeyBlobFormat.RawPrivateKey);
        //}

        public byte[] GetPublicKey()
        {

            return _key.Export(KeyBlobFormat.RawPublicKey);
        }

        public bool VerifySignature(byte[] signature, byte[] data, byte[] pubKey)
        {

            PublicKey newPubKey = PublicKey.Import(_algorithm, pubKey, KeyBlobFormat.RawPublicKey);
            return _algorithm.Verify(newPubKey, data, signature);
        }

        public static bool VerifySignatureIsolated(byte[] signature, byte[] data, byte[] pubKey)
        {
            PublicKey newPubKey = PublicKey.Import(SignatureAlgorithm.Ed25519, pubKey, KeyBlobFormat.RawPublicKey);
            return SignatureAlgorithm.Ed25519.Verify(newPubKey, data, signature);
        }

        public static bool VerifyPublicKey(byte[] pubKeyMaybe)
        {
            return PublicKey.TryImport(SignatureAlgorithm.Ed25519, pubKeyMaybe, KeyBlobFormat.RawPublicKey, out _);
        }
    }
}
