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
        private AsymmetricKeyParameter PubKey;
        private AsymmetricKeyParameter PrivKey;


        internal HomeKeys()
        {
            var random = new SecureRandom();
            var keyGenParameters = new DilithiumKeyGenerationParameters(random, DilithiumParameters.Dilithium5);
            var keyPairGenerator = new DilithiumKeyPairGenerator();

            keyPairGenerator.Init(keyGenParameters);

            var keyPair = keyPairGenerator.GenerateKeyPair();

            PubKey = keyPair.Public;
            PrivKey = keyPair.Private;

        }


        internal AsymmetricKeyParameter GetPublic()
        {
            return PubKey;
        }

        internal AsymmetricKeyParameter GetPrivate()
        {
            return PrivKey;
        }

    }
}
