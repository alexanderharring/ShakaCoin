using Microsoft.Testing.Platform.Extensions.Messages;
using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin
{
    internal class MainLogic
    {
        private FileManagement _fm;
        private Wallet _wallet;

        public string WalletName;

        private bool ShouldGenerateNewWallet = false;

        internal MainLogic()
        {
            Console.WriteLine("Running Shakacoin.");

            _fm = new FileManagement();

            WalletName = GetWalletName();

            Console.WriteLine("Loading Wallet: " + WalletName);

            if (ShouldGenerateNewWallet)
            {
                _wallet = new Wallet(WalletName);

                _fm.AddWallet(WalletName, _wallet.GetPK());
            } else
            {
                _wallet = new Wallet(_fm.ReadWallet(WalletName), WalletName);
            }

            MainLoop();

        }

        private void DisplayWalletData()
        {
            Console.Write("Loaded Wallet: [ " + WalletName + " ] ");
            Console.WriteLine(" - Wallet PublicKey: " + Hasher.GetHexStringQuick(_wallet.GetPublicKey()));
            Console.WriteLine("");
        }

        private void ClearScreen()
        {
            Console.Clear();
        }

        public void MainOptions()
        {
            Console.WriteLine("ShakaCoin Options: ");
            Console.WriteLine("Press [B] to find an account balance");
            Console.WriteLine("Press [T] to make a transaction");
            Console.WriteLine("Press [V] to view blockchain");
            Console.WriteLine("Press [S] to find a transaction");
            Console.WriteLine("Press [M] to start mining");
            Console.WriteLine("Press [X] to exit software.\n");
        }

        private void WalletOptions()
        {
            Console.WriteLine("Wallet Options: ");
            Console.WriteLine("Press [L] to list wallets");
            Console.WriteLine("Press [N] to generate a new wallet");
            Console.WriteLine("Press [O] to load a wallet");
            Console.WriteLine("Press [X] to exit software.\n");
        }

        private string GetWalletName()
        {

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

                        List<string> wallts = _fm.GetAllWallets();

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
                            byte[] pk = _fm.ReadWallet(wltName);

                            if (pk.Length > 0)
                            {
                                ShouldGenerateNewWallet = false;
                                return wltName;
                                
                            } else
                            {
                                Console.Write("Wallet not found.");
                            }
                            Console.ReadLine();
                        }
                    }

                    else if (line.ToLower()[0] == 'n')
                    {
                        ClearScreen();
                        Console.WriteLine("Enter a name for your new wallet:");
                        string? wltName = Console.ReadLine();

                        if ((wltName != null) && (wltName.Length > 0))
                        {
                            ShouldGenerateNewWallet = true;
                            return wltName;
                        } else
                        {
                            Console.Write("Enter a correct name!");
                            Console.ReadLine();
                        }

                    }


                }

                
            }

            return "";

        }

        private ulong GetAccountBalance(byte[] pk)
        {
            return (ulong)1234;
        }

        private void MainLoop()
        {
            bool running = true;

            while (running)
            {
                ClearScreen();
                DisplayWalletData();

                MainOptions();

                string? line = Console.ReadLine();

                if ((line != null) && (line.Length > 0))
                {

                    if (line.ToLower()[0] == 'x')
                    {
                        Environment.Exit(0);
                        return;
                    }

                    else if (line.ToLower()[0] == 'b') // get balance
                    {
                        ClearScreen();
                        DisplayWalletData();
                        Console.WriteLine("Finding an account balance.");
                        Console.Write("Press [ Y ] to get your own balance. Or [ N ] for an another account's balance. ");

                        string? inputTwo = Console.ReadLine();

                        if ((inputTwo != null) && (inputTwo.Length > 0))
                        {

                            if (inputTwo.ToLower()[0] == 'y')
                            {
                                ulong bal = GetAccountBalance(_wallet.GetPublicKey());
                                ClearScreen();
                                DisplayWalletData();
                                Console.WriteLine("Your account balance is: " + bal.ToString());
                                Console.ReadLine();
                            }
                            else
                            {
                                var isBadKey = true;
                                byte[] pk = new byte[32];

                                while (isBadKey)
                                {
                                    ClearScreen();
                                    DisplayWalletData();
                                    Console.Write("Enter the public key of the account you want to find the balance of: ");
                                    string? inputThree = Console.ReadLine();

                                    if ((inputThree != null) && (inputThree.Length > 0))
                                    {
                                        byte[] byts = Hasher.GetBytesFromHexStringQuick(inputThree);

                                        if (Wallet.VerifyPublicKey(byts))
                                        {
                                            isBadKey = false;
                                            pk = byts;
                                        }
                                    }

                                }

                                ulong bal = GetAccountBalance(pk);

                                Console.WriteLine("Account balance: " + bal.ToString() + " of " + Hasher.GetHexStringQuick(pk));
                                Console.ReadLine();
                            }

                        }
                    }

                    else if (line.ToLower()[0] == 't') // make transaction
                    {

                    }

                    else if (line.ToLower()[0] == 'v') // view blockchain
                    {

                    }

                    else if (line.ToLower()[0] == 's') // search for a transaction
                    {

                    }

                    else if (line.ToLower()[0] == 'm') // start mining
                    {

                    }

                }



            }
        }

    }
}
