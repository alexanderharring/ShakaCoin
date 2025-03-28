using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

namespace ShakaCoin.MainInteraction
{
    internal class UtilConsole
    {

        public static void WalletOptions()
        {
            Console.WriteLine("Wallet Options: ");
            Console.WriteLine("Press [L] to list wallets");
            Console.WriteLine("Press [N] to generate a new wallet");
            Console.WriteLine("Press [O] to load a wallet");
            Console.WriteLine("Press [X] to exit software.\n");
        }

        public static void ClearScreen()
        {
            Console.Clear();
        }

        public static void MainOptions(bool isValidator)
        {
            Console.WriteLine("ShakaCoin Options: ");
            Console.WriteLine("Press [B] to find an account balance");
            Console.WriteLine("Press [T] to make a transaction");
            Console.WriteLine("Press [K] to view connected nodes");
            Console.WriteLine("Press [G] to get UTXOs for your wallet");
            if (isValidator)
            {
                Console.WriteLine("Press [V] to view blockchain");
                Console.WriteLine("Press [S] to pull up an UTXO");
                Console.WriteLine("Press [M] to start mining");
                Console.WriteLine("Press [Q] to swap to verifier node");
            } else
            {
                Console.WriteLine("Press [Q] to swap to validator node");
            }
            
            Console.WriteLine("Press [X] to exit software.\n");
        }
    }
}
