    using Microsoft.Testing.Platform.Extensions.Messages;
using ShakaCoin.Blockchain;
using ShakaCoin.Networking;
using ShakaCoin.PaymentData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.MainInteraction
{
    internal class MainLogic
    {
        private FileManagement _fm;
        private Wallet _wallet;

        public string WalletName;

        private bool ShouldGenerateNewWallet = false;

        private PeerManager? _peerManager;
        private BlockchainHandler _blockchainHandler;

        private bool _isValidatorNode = false;

        internal MainLogic()
        {
            Console.WriteLine("Running Shakacoin.");

            _fm = FileManagement.Instance;


            WalletName = GetWalletName();

            Console.WriteLine("Loading Wallet: " + WalletName);

            if (ShouldGenerateNewWallet)
            {
                _wallet = new Wallet(WalletName);

                _fm.AddWallet(WalletName, _wallet.GetPK());
            }
            else
            {
                _wallet = new Wallet(_fm.ReadWallet(WalletName), WalletName);
            }



            _blockchainHandler = new BlockchainHandler(_isValidatorNode);
            _blockchainHandler.MinerPubKey = _wallet.GetPublicKey();
            _blockchainHandler.CheckBlockchain();

            _ = RunBootstrap();

            MainLoopAsync();



        }


        private void DisplayWalletData()
        {
            if (_isValidatorNode)
            {
                Console.Write("Validator Node - ");
            } else
            {
                Console.Write("Verifier Node - ");
            }
            Console.Write("Loaded Wallet: [ " + WalletName + " ] ");
            Console.WriteLine(" - Wallet PublicKey: " + Hasher.GetHexStringQuick(_wallet.GetPublicKey()));
            Console.WriteLine("");
        }


        private string GetWalletName()
        {

            bool DoesNotHaveWalletYet = true;

            while (DoesNotHaveWalletYet)
            {
                UtilConsole.ClearScreen();
                UtilConsole.WalletOptions();

                string? line = Console.ReadLine();

                if (line != null && line.Length > 0)
                {

                    if (line.ToLower()[0] == 'x')
                    {
                        Environment.Exit(0);
                        return "";
                    }

                    else if (line.ToLower()[0] == 'l')
                    {

                        List<string> wallts = _fm.GetAllWallets();

                        Console.WriteLine("\nListing " + wallts.Count + " wallets:");
                        foreach (string w in wallts)
                        {
                            Console.WriteLine(w);
                        }

                        Console.Write("Press any key to continue...");
                        Console.ReadLine();
                    }

                    else if (line.ToLower()[0] == 'o')
                    {
                        UtilConsole.ClearScreen();
                        Console.WriteLine("Wallet Options:");
                        List<string> wallts = _fm.GetAllWallets();

                        foreach (string w in wallts)
                        {
                            Console.WriteLine(w);
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Enter wallet name:");
                        string? wltName = Console.ReadLine();

                        if (wltName != null && wltName.Length > 0)
                        {
                            byte[] pk = _fm.ReadWallet(wltName);

                            if (pk.Length > 0)
                            {
                                ShouldGenerateNewWallet = false;
                                return wltName;

                            }
                            else
                            {
                                Console.Write("Wallet not found.");
                            }
                            Console.ReadLine();
                        }
                    }

                    else if (line.ToLower()[0] == 'n')
                    {
                        UtilConsole.ClearScreen();
                        Console.WriteLine("Enter a name for your new wallet:");
                        string? wltName = Console.ReadLine();

                        if (wltName != null && wltName.Length > 0)
                        {
                            ShouldGenerateNewWallet = true;
                            return wltName;
                        }
                        else
                        {
                            Console.Write("Enter a correct name!");
                            Console.ReadLine();
                        }

                    }


                }


            }

            return "";

        }


        private void MainLoopAsync()
        {
            bool running = true;

            while (running)
            {
                UtilConsole.ClearScreen();
                DisplayWalletData();

                UtilConsole.MainOptions(_isValidatorNode);

                _peerManager._isOnNetworkDebug = false;

                _blockchainHandler.CheckBlockchain();

                string? line = Console.ReadLine();

                if (line != null && line.Length > 0)
                {

                    if (line.ToLower()[0] == 'x')
                    {
                        Environment.Exit(0);
                        return;
                    }

                    else if (line.ToLower()[0] == 'k')
                    {
                        UtilConsole.ClearScreen();
                        DisplayWalletData();
                        List<string> ips = _peerManager.ListPeerIps();
                        Console.WriteLine("Listing " + ips.Count +" peers");
                        for (int i=0; i<ips.Count;i++)
                        {
                            Console.WriteLine("#" + i.ToString() + " - " + ips[i]);
                        }
                        Console.WriteLine();
                        _peerManager._isOnNetworkDebug = true;

                        Console.ReadLine();
                    }
                    else if (line.ToLower()[0] == 'g')
                    {
                        UtilConsole.ClearScreen();
                        DisplayWalletData();

                        List<byte[]> outpoints = _fm.GetUTXOsForPK(_wallet.GetPublicKey());
                        Console.WriteLine("Listing " + outpoints.Count + " UTXOs:");
                        for (int j=0; j < outpoints.Count; j++)
                        {
                            Console.WriteLine("#" + j.ToString() + " - " + Hasher.GetHexStringQuick(outpoints[j]));
                        }
                        Console.ReadLine();
                    }

                    else if (line.ToLower()[0] == 'b') // get balance
                    {
                        UtilConsole.ClearScreen();
                        DisplayWalletData();
                        Console.WriteLine("Finding an account balance.");
                        Console.Write("Press [ Y ] to get your own balance. Or [ N ] for an another account's balance. ");

                        string? inputTwo = Console.ReadLine();

                        if (inputTwo != null && inputTwo.Length > 0)
                        {

                            if (inputTwo.ToLower()[0] == 'y')
                            {
                                ulong bal = _fm.GetAccountBalance(_wallet.GetPublicKey());
                                UtilConsole.ClearScreen();
                                DisplayWalletData();
                                Console.WriteLine("Your account balance is: " + bal.ToString());
                                Console.ReadLine();
                            }
                            else if(inputTwo.ToLower()[0] == 'n')
                            {
                                var isBadKey = true;
                                byte[] pk = new byte[32];

                                while (isBadKey)
                                {
                                    UtilConsole.ClearScreen();
                                    DisplayWalletData();
                                    Console.Write("Enter the public key of the account you want to find the balance of: ");
                                    string? inputThree = Console.ReadLine();

                                    if (inputThree != null && inputThree.Length > 0)
                                    {
                                        byte[] byts = Hasher.GetBytesFromHexStringQuick(inputThree);

                                        if (Wallet.VerifyPublicKey(byts))
                                        {
                                            isBadKey = false;
                                            pk = byts;
                                        }
                                    }

                                }

                                ulong bal = _fm.GetAccountBalance(pk);

                                Console.WriteLine("Account balance: " + bal.ToString() + " of " + Hasher.GetHexStringQuick(pk));
                                Console.ReadLine();
                            }

                        }
                    }

                    else if (line.ToLower()[0] == 't') // make transaction
                    {
                        UtilConsole.ClearScreen();
                        DisplayWalletData();
                        Console.WriteLine("Creating a new transaction");
                        Console.Write("What is your destination public key: ");

                        string? pubK = Console.ReadLine();
                        byte[] pubKbytes = Hasher.GetBytesFromHexStringQuick(pubK);
                        if (!HomeKeys.VerifyPublicKey(pubKbytes))
                        {
                            Console.WriteLine("Invalid public key.");
                            Console.ReadLine();
                            
                        } else
                        {
                            UtilConsole.ClearScreen();
                            DisplayWalletData();
                            Console.WriteLine("Creating a new transaction");
                            Console.WriteLine("Sending to: " + pubK);
                            Console.Write("How much Shakacoin would you like to send: ");
                            string? amnt = Console.ReadLine();

                            if (ulong.TryParse(amnt, out ulong res))
                            {
                                UtilConsole.ClearScreen();
                                DisplayWalletData();
                                Console.WriteLine("Creating a new transaction");
                                Console.WriteLine("Sending " + res.ToString() + " Shakacoin to: " + pubK);
                                Console.Write("How much Shakacoin would you like to add as mining fee");
                                string? feeAmnnt = Console.ReadLine();

                                if (ulong.TryParse(feeAmnnt, out ulong feeAmount))
                                {
                                    if (feeAmount == 0)
                                    {
                                        Console.WriteLine("Fee must be greater than 0.");
                                    } else
                                    {
                                        UtilConsole.ClearScreen();
                                        DisplayWalletData();
                                        Console.WriteLine("Creating a new transaction");
                                        Console.WriteLine("Sending " + res.ToString() + " Shakacoin to: " + pubK);
                                        Console.WriteLine("You are leaving " + feeAmount.ToString() + " Shakacoin as a mining fee? ");
                                        Console.Write("Ensure everything is correct. Press [Y] to accept. ");
                                        string? confirmInput = Console.ReadLine();

                                        if (!(confirmInput is null))
                                        {
                                            if (confirmInput.ToLower()[0] == 'y')
                                            {
                                                Console.WriteLine("Sending transaction...");
                                                Transaction generatedTransaction = _wallet.GenerateTransaction(pubKbytes, res, feeAmount);

                                                if (generatedTransaction is null)
                                                {
                                                    Console.WriteLine("Could not form a transaction.");
                                                } else
                                                {
                                                    Console.WriteLine("Transaction generation successful. Distributing to network");

                                                    _ = _peerManager.DiffuseTransaction(generatedTransaction, NetworkConstants.HopCount);
                                                }

                                                Console.ReadLine();
                                                
                                            } else
                                            {
                                                Console.WriteLine("Transaction aborted.");
                                            }
                                            
                                        } else
                                        {
                                            Console.WriteLine("Transaction aborted.");
                                        }
                                        Console.ReadLine();

                                    }
                                } else
                                {
                                    Console.WriteLine("Invalid amount.");
                                    Console.ReadLine();
                                }


                            } else
                            {
                                Console.WriteLine("Invalid amount.");
                                Console.ReadLine();
                            }
                        }
                    }

                    else if (line.ToLower()[0] == 'v') // view blockchain
                    {
                        uint blockInd = 0;
                        UtilConsole.ClearScreen();
                        DisplayWalletData();
                        
                        while (!(FileManagement.ReadBlock(blockInd) is null))
                        {
                            Block blk = Parser.ParseBlock(FileManagement.ReadBlock(blockInd));
                            Console.WriteLine("Block #" + blockInd.ToString() + " exists. There are " + blk.Transactions.Count.ToString() + " transactions in this block.");
                            blockInd++;
                        }
                        Console.Write("\nEnter the block number you would like to inspect.  ");
                        string? xin = Console.ReadLine();
                        if (int.TryParse(xin, out int goodBlockNum))
                        {

                            if (!(FileManagement.ReadBlock((uint)goodBlockNum) is null))
                            {

                                Block blk = Parser.ParseBlock(FileManagement.ReadBlock((uint)goodBlockNum));
                                UtilConsole.ClearScreen();
                                DisplayWalletData();

                                Console.WriteLine("Block #" + blk.BlockHeight.ToString());
                                Console.WriteLine("There are " + blk.Transactions.Count.ToString() + " transactions\n");

                                for (int j=0; j < blk.Transactions.Count; j++)
                                {
                                    Transaction tx = blk.Transactions[j];
                                    Console.WriteLine("Tx #" + j.ToString());
                                    
                                    for (int k=0; k<tx.Inputs.Count; k++)
                                    {
                                        Console.WriteLine("   Input #" + k.ToString() + " - Outpoint: " + Hasher.GetHexStringQuick(tx.Inputs[k].Outpoint));
                                    }
                                    for (int k = 0; k < tx.Outputs.Count; k++)
                                    {
                                        Console.WriteLine("   Output #" + k.ToString() + " - PubKey: " + Hasher.GetHexStringQuick(tx.Outputs[k].DestinationPublicKey));
                                    }
                                    Console.WriteLine();
                                }

                                Console.ReadLine();

                            } else
                            {
                                Console.Write("\nThat block is not found");
                                Console.ReadLine();
                            }


                        } else
                        {
                            Console.Write("\nEnter a number only.");
                            Console.ReadLine();
                        }
                        

                    }

                    else if (line.ToLower()[0] == 's') // search for a utxo
                    {

                    }

                    else if (line.ToLower()[0] == 'm') // start mining
                    {
                        UtilConsole.ClearScreen();
                        DisplayWalletData();

                        _blockchainHandler.SetMiningAsync(true);

                        string? inpt = Console.ReadLine();
                        _blockchainHandler.SetMiningAsync(false);
                        Console.ReadLine();
                    }

                    else if (line.ToLower()[0] == 'q') // swap
                    {
                        _isValidatorNode = !_isValidatorNode;
                        _blockchainHandler.UpdateNodeStatus(_isValidatorNode);
                    }

                }



            }
        }

        public async Task RunBootstrap()
        {

            _peerManager = new PeerManager(true);
            
            _peerManager._blockchainHandler = _blockchainHandler;
            _blockchainHandler._peerManager = _peerManager;

            await _peerManager.Start();
        }

        public async Task RunPeerNode()
        {
            _peerManager = new PeerManager();

            _peerManager._blockchainHandler = _blockchainHandler;
            _blockchainHandler._peerManager = _peerManager;

            await _peerManager.ConnectToBootstrapNode();
            await _peerManager.Start();

        }

    }
}
