using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Testing.Platform.Extensions.Messages;
using NSec.Cryptography;
using ShakaCoin.Datastructures;
using ShakaCoin.PaymentData;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShakaCoin.Blockchain
{
    public class FileManagement
    {
        private static string _appName = "ShakaCoin";
        private static string _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);
        private static string BlockDir = Path.Combine(_appDataPath, "Blocks");
        private static string BlockHeaderDir = Path.Combine(_appDataPath, "BlockHeaders");
        private static string OutputsDir = Path.Combine(_appDataPath, "Outputs");
        private static string WalletsDir = Path.Combine(_appDataPath, "Wallets");
        private static string OutputBFsDir = Path.Combine(_appDataPath, "OutputBFs");

        private DatabaseInteraction _outputDB;

        private static FileManagement _FileManagement;

        public uint maxBlockNum = 0;

        public static FileManagement Instance
        {
            get
            {
                if (_FileManagement == null)
                {
                    _FileManagement = new FileManagement();
                }

                return _FileManagement;
            }
        }

        private FileManagement()
        {
            if (!Directory.Exists(_appDataPath))
            {
                Console.WriteLine("No ShakaCoin Directory Detected. Creating one...");
                Directory.CreateDirectory(_appDataPath);
            }
            if (!Directory.Exists(BlockDir))
            {
                Directory.CreateDirectory(BlockDir);
            }

            if (!Directory.Exists(OutputsDir))
            {
                Directory.CreateDirectory(OutputsDir);
            }

            if (!Directory.Exists(WalletsDir))
            {
                Directory.CreateDirectory(WalletsDir);
            }
            if (!Directory.Exists(OutputBFsDir))
            {
                Directory.CreateDirectory(OutputBFsDir);
            }
            if (!Directory.Exists(BlockHeaderDir))
            {
                Directory.CreateDirectory(BlockHeaderDir);
            }

            _outputDB = new DatabaseInteraction(OutputsDir);
            CheckGenesisBlock();
        }

        public void WriteBlock(Block block)
        {
            string fName = "b" + block.BlockHeight.ToString() + ".dat";
            string fileP = Path.Combine(BlockDir, fName);

            File.WriteAllBytes(fileP, block.GetBlockBytes());

            foreach (Transaction tx in block.Transactions)
            {
                AddOutpointsToDB(tx, block.BlockHeight);
            }
        }

        public void AddOutpointsToDB(Transaction tx, uint bHeight)
        {
            for (int i = 0; i < tx.Outputs.Count; i++)
            {
                byte[] key = new byte[33];

                Buffer.BlockCopy(Hasher.Hash256(tx.GetBytes()), 0, key, 0, 32);
                key[32] = (byte)i;

                byte[] value = new byte[44]; // block height (4) + amount (8) + pub key ( 32 ) 
                Buffer.BlockCopy(BitConverter.GetBytes(bHeight), 0, value, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(tx.Outputs[i].Amount), 0, value, 4, 8);
                Buffer.BlockCopy(tx.Outputs[i].DestinationPublicKey, 0, value, 12, 32);

                _outputDB.AddValue(key, value);

                
            }
        }

        public void ClearInputsFromDB(Transaction tx)
        {
            foreach (Input ix in tx.Inputs)
            {
                _outputDB.RemoveValue(ix.Outpoint);
            }
        }

        public List<(Input, byte[], ulong)> GetInputsForTransaction(byte[] pubkey, ulong amountReq)
        {
            ulong bal = 0;
            List<(Input, byte[], ulong)> inputList = new List<(Input, byte[], ulong)>();

            for (uint i = 0; i < maxBlockNum; i++)
            {

                byte[] obfBytes = GetOutputBF(i);
                OutputBloomFilter obf = Parser.ParseBloomFilter(obfBytes);

                if (obf.ProbablyContains(pubkey))
                {

                    Block blk = Parser.ParseBlock(ReadBlock(i));

                    foreach (Transaction tx in blk.Transactions)
                    {

                        for (int oxI = 0; oxI < tx.Outputs.Count; oxI++)
                        {
                            if (Hasher.GetHexStringQuick(pubkey) == Hasher.GetHexStringQuick(tx.Outputs[oxI].DestinationPublicKey))
                            {
                                byte[] key = new byte[33];

                                Buffer.BlockCopy(Hasher.Hash256(tx.GetBytes()), 0, key, 0, 32);
                                key[32] = (byte)oxI;


                                if (!(DBGetValue(key) is null))
                                {

                                    bal += tx.Outputs[oxI].Amount;
                                    Input newInput = new Input(Hasher.Hash256(tx.GetBytes()), (byte)oxI);
                                    inputList.Add((newInput, key, tx.Outputs[oxI].Amount));

                                    if (bal > amountReq)
                                    {
                                        return inputList;
                                    }
                                }
                            }

                        }
                    }

                }
            }

            return null;
        }

        public void WriteHeader(byte[] data, uint bHeight)
        {
            if (data.Length != 117)
            {
                throw new ArgumentException();
            }
            uint fileInd = bHeight / 64;
            string fName = "h" + fileInd.ToString() + ".dat";
            string fileP = Path.Combine(BlockHeaderDir, fName);

            if (!(File.Exists(fileP)))
            {
                File.Create(fileP);

                File.WriteAllBytes(fileP, new byte[117 * 64]);
            }

            using (FileStream fs = File.OpenWrite(fileP))
            {
                int ind = (int)(bHeight % 64);
                fs.Write(data, ind, 117);
            }
        }

        public byte[] GetHeader(uint bHeight)
        {
            uint fileInd = bHeight / 64;
            string fName = "h" + fileInd.ToString() + ".dat";
            string fileP = Path.Combine(BlockHeaderDir, fName);

            if (!(File.Exists(fileP)))
            {
                return null;
            }

            using (FileStream fs = File.OpenRead(fileP))
            {
                byte[] buffer = new byte[117];
                int ind = (int)(bHeight % 64);
                fs.Read(buffer, ind, 117);

                return buffer;
            }

        }

        public void WriteOutputBF(byte[] data, uint bHeight)
        {
            if (data.Length != 128)
            {
                throw new ArgumentException();
            }
            uint fileInd = bHeight / 64;
            string fName = "o" + fileInd.ToString() + ".dat";
            string fileP = Path.Combine(OutputBFsDir, fName);

            if (!(File.Exists(fileP)))
            {
                File.WriteAllBytes(fileP, new byte[8192]);
            }
            

            using (FileStream fs = File.OpenWrite(fileP))
            {
                int ind = (int)(bHeight % 64);
                fs.Position = ind * 128;
                fs.Write(data, 0, 128);
            }
        }

        public byte[] GetOutputBF(uint bHeight)
        {
            uint fileInd = bHeight / 64;
            string fName = "o" + fileInd.ToString() + ".dat";
            string fileP = Path.Combine(OutputBFsDir, fName);

            if (!(File.Exists(fileP)))
            {
                return null;
            }

            using (FileStream fs = File.OpenRead(fileP))
            {
                byte[] buffer = new byte[128];
                int ind = (int)(bHeight % 64);
                fs.Position = ind * 128;
                fs.Read(buffer, 0, 128);

                return buffer;
            }

        }

        public static byte[] ReadBlock(uint height)
        {
            string fName = "b" + height.ToString() + ".dat";
            string fileP = Path.Combine(BlockDir, fName);


            try
            {
                return File.ReadAllBytes(fileP);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public void CheckGenesisBlock()
        {
            if (ReadBlock(0) is null) // no genesis block
            {
                Console.WriteLine("Writing genesis block...");
                Block gb = GenesisBlock.MakeGenesisBlock();

                WriteBlock(gb);
            } else
            {
                Console.WriteLine("Gb already exists");
            }
        }

        public void TestFile()
        {
            string filep = Path.Combine(_appDataPath, "test.txt");
            File.WriteAllText(filep, "TestData");

        }

        public bool VerifyOutput(byte[] ox)
        {
            return (_outputDB.GetValue(ox) != null);
        }

        public (ulong, byte[])? RetrieveOutpointData(byte[] outpoint)
        {


            byte[] outpointData = _outputDB.GetValue(outpoint);

            if (outpointData is null)
            {
                return null;
            }

            ulong amount = BitConverter.ToUInt64(outpointData, 4);
            byte[] pk = new byte[32];

            Buffer.BlockCopy(outpointData, 12, pk, 0, 32);

            return (amount, pk);
        }

        public void DBAddValue(byte[] key, byte[] value)
        {
            _outputDB.AddValue(key, value);
        }

        public void DBRemoveValue(byte[] key)
        {
            _outputDB.RemoveValue(key);
        }

        public byte[] DBGetValue(byte[] key)
        {
            return _outputDB.GetValue(key);
        }

        public void DBCLose()
        {
            _outputDB.Close();
        }

        public void AddWallet(string name, byte[] pk)
        {
            string fName = "w_" + name + ".dat";
            string fileP = Path.Combine(WalletsDir, fName);

            File.WriteAllBytes(fileP, pk);
        }

        public ulong GetAccountBalance(byte[] pk)
        {
            ulong bal = 0;

            for (uint i=0; i<= maxBlockNum; i++)
            {

                byte[] obfBytes = GetOutputBF(i);

                OutputBloomFilter obf = Parser.ParseBloomFilter(obfBytes);

                if (obf.ProbablyContains(pk))
                {

                    Block blk = Parser.ParseBlock(ReadBlock(i));

                    foreach (Transaction tx in blk.Transactions)
                    {
                        for (int j = 0; j < tx.Outputs.Count; j++)
                        {
                            Output ox = tx.Outputs[j];

                            if (Hasher.GetHexStringQuick(pk) == Hasher.GetHexStringQuick(ox.DestinationPublicKey))
                            {
                                byte[] outpoint = new byte[33];
                                Buffer.BlockCopy(Hasher.Hash256(tx.GetBytes()), 0, outpoint, 0, 32);
                                outpoint[32] = (byte)j;

                                if (!(DBGetValue(outpoint) is null))
                                {
                                    bal += ox.Amount;
                                }

                            }
                        }

                    }

                }
            }

            return bal;
        }

        public List<byte[]> GetUTXOsForPK(byte[] pubKey)
        {
            List<byte[]> result = new List<byte[]>();

            for (uint i = 0; i <= maxBlockNum; i++)
            {

                byte[] obfBytes = GetOutputBF(i);


                OutputBloomFilter obf = Parser.ParseBloomFilter(obfBytes);

                if (obf.ProbablyContains(pubKey))
                {

                    Block blk = Parser.ParseBlock(ReadBlock(i));

                    foreach (Transaction tx in blk.Transactions)
                    {
                        for (int j = 0; j<tx.Outputs.Count; j++)
                        {
                            Output ox = tx.Outputs[j];

                            if (Hasher.GetHexStringQuick(pubKey) == Hasher.GetHexStringQuick(ox.DestinationPublicKey))
                            {
                                byte[] outpoint = new byte[33];
                                Buffer.BlockCopy(Hasher.Hash256(tx.GetBytes()), 0, outpoint, 0, 32);
                                outpoint[32] = (byte)j;

                                if (!(DBGetValue(outpoint) is null))
                                {
                                    result.Add(outpoint);
                                }
                                
                            }
                        }

                    }

                }
            }

            return result;
        }

        public byte[] ReadWallet(string name)
        {
            string fName = "w_" + name + ".dat";
            string fileP = Path.Combine(WalletsDir, fName);

            try
            {
                return File.ReadAllBytes(fileP);
            }
            catch (FileNotFoundException)
            {
                
                
            }

            return [];


        }

        public List<string> GetAllWallets()
        {
            string[] ts = Directory.GetFiles(WalletsDir, "*.dat");

            List<string> wltsList = ts.ToList();

            for (int i=0; i < ts.Length; i++)
            {
                wltsList[i] = wltsList[i].Replace(WalletsDir+"\\", "");
                wltsList[i] = wltsList[i].Replace(".dat", "");
                wltsList[i] = wltsList[i].Substring(2);
            }

            return wltsList;
        }

    }
}
