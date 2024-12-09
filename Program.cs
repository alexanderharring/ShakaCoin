using ShakaCoin.Cryptography;

namespace ShakaCoin
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainCryptography mainCryptography = new MainCryptography();

            var message = "Hello senny";

            var sig = mainCryptography.SignSignature(mainCryptography.GetPrivateKey(), message);

            var ver = mainCryptography.VerifySignature(mainCryptography.GetPublicKey(), message, sig);

            Console.WriteLine(ver);
        }
    }
}
