using Microsoft.Testing.Platform.Extensions.Messages;
using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin
{
    internal class MainLogic
    {
        internal static void ClearScreen()
        {
            Console.Clear();
        }

        internal static void WalletOptions()
        {
            Console.WriteLine("Wallet Options: ");
            Console.WriteLine("Press [L] to list wallets");
            Console.WriteLine("Press [N] to generate a new wallet");
            Console.WriteLine("Press [O] to load a wallet");
            Console.WriteLine("Press [X] to exit software.");
        }

        internal static void Initiate()
        {
            string walletName = GetWalletName();

            Console.WriteLine("Loading Wallet: " + walletName);
        }

        internal static string GetWalletName()
        {

            FileManagement fm = new FileManagement();

            Console.WriteLine("Running Shakacoin.");


            bool DoesNotHaveWalletYet = true;

            while (DoesNotHaveWalletYet)
            {
                ClearScreen();
                WalletOptions();

                string? line = Console.ReadLine();
                
                if ((line != null) && (line.Length > 0))
                {

                    if (line.ToLower()[0] == 'x')
                    {
                        Environment.Exit(0);
                        return "";
                    } 
                    
                    else if (line.ToLower()[0] == 'l')
                    {

                        List<string> wallts = fm.GetAllWallets();

                        foreach (string w in wallts)
                        {
                            Console.WriteLine(w);
                        }

                        Console.Write("Press any key to continue...");
                        Console.ReadLine();
                    }

                    else if (line.ToLower()[0] == 'o')
                    {
                        ClearScreen();
                        Console.WriteLine("Enter wallet name:");
                        string? wltName = Console.ReadLine();
                        
                        if ((wltName != null) && (wltName.Length > 0))
                        {
                            byte[] pk = fm.ReadWallet(wltName);

                            if (pk.Length > 0)
                            {
                                return wltName;
                                
                            } else
                            {
                                Console.Write("Wallet not found.");
                            }
                            Console.ReadLine();
                        }
                    }


                }

                
            }

            return "";

        }

    }
}
