using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Org.BouncyCastle.Utilities;

namespace ShakaCoin.Cryptography
{
    internal class HomeKeys
    {
        private DilithiumPublicKeyParameters PubKey;
        private DilithiumPrivateKeyParameters PrivKey;


        internal HomeKeys()
        {
            var random = new SecureRandom();
            var keyGenParameters = new DilithiumKeyGenerationParameters(random, DilithiumParameters.Dilithium5);
            var keyPairGenerator = new DilithiumKeyPairGenerator();

            keyPairGenerator.Init(keyGenParameters);

            var keyPair = keyPairGenerator.GenerateKeyPair();

            PubKey = (DilithiumPublicKeyParameters)keyPair.Public;
            PrivKey = (DilithiumPrivateKeyParameters)keyPair.Private;


        }

        internal AsymmetricKeyParameter GetRawPublic()
        {
            return PubKey;
        }

        internal AsymmetricKeyParameter GetRawPrivate()
        {
            return PrivKey;
        }

        internal string GetPublic()
        {
            return Convert.ToHexString(PubKey.GetEncoded());
        }

        internal string GetPrivate()
        {
            return Convert.ToHexString(PrivKey.GetEncoded());
        }

        internal void PrintKey(bool isPub)
        {
            Console.WriteLine(PubKey.GetEncoded().Length);

            if (isPub)
            {
                Console.WriteLine(GetPublic());
            } else
            {
                Console.WriteLine(GetPrivate());
            }

        }

    }
}
