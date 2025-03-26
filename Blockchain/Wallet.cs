using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.PaymentData;

namespace ShakaCoin.Blockchain
{
    public class Wallet
    {

        private HomeKeys _hk;

        private string walletName;

        public Wallet(byte[] privKey, string name)
        {
            _hk = new HomeKeys(privKey);
            walletName = name;
        }

        public Wallet(string name)
        {
            _hk = new HomeKeys();
            walletName = name;
        }

        public string GetWalletName()
        {
            return walletName;
        }

        public byte[] GetPublicKey()
        {
            return _hk.GetPublicKey();
        }

        public byte[] GetPK()
        {
            return _hk.GetPrivateKey();
        }

        public static bool VerifySignature(byte[] signature, byte[] data, byte[] pubKey)
        {
            return HomeKeys.VerifySignatureIsolated(signature, data, pubKey);
        }

        public byte[] SignData(byte[] data)
        {
            return _hk.SignData(data);
        }

        public static bool VerifyPublicKey(byte[] publicKey)
        {
            return HomeKeys.VerifyPublicKey(publicKey);
        }

        public Transaction GenerateTransaction(byte[] pubK, ulong mainAmount, ulong minerFee)
        {
            Transaction buildTx = new Transaction(0x00);

            Output buildOX = new Output(mainAmount, pubK);
        }
    }

}
