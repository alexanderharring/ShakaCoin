using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ShakaCoin.PaymentData;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShakaCoin.Blockchain
{
    public class FileManagement
    {
        private static string _appName = "ShakaCoin";
        private static string _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);
        private static string BlockDir = Path.Combine(_appDataPath, "Blocks");
        private static string OutputsDir = Path.Combine(_appDataPath, "Outputs");
        private static string WalletsDir = Path.Combine(_appDataPath, "Wallets");

        private DatabaseInteraction _outputDB;

        public FileManagement()
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

            _outputDB = new DatabaseInteraction(OutputsDir);
        }

        public void WriteBlock(Block block)
        {
            string fName = "b" + block.BlockHeight.ToString() + ".dat";
            string fileP = Path.Combine(BlockDir, fName);

            File.WriteAllBytes(fileP, block.GetBlockBytes());

            foreach (Transaction tx in block.Transactions)
            {
                for (int i = 0; i < tx.Outputs.Count;  i++)
                {
                    byte[] key = new byte[33];

                    Buffer.BlockCopy(Hasher.Hash256(tx.GetBytes()), 0, key, 0, 32);
                    key[32] = (byte)i;

                    byte[] value = new byte[44]; // block height (4) + amount (8)
                    Buffer.BlockCopy(BitConverter.GetBytes(block.BlockHeight), 0, value, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(tx.Outputs[i].Amount), 0, value, 4, 8);
                    Buffer.BlockCopy(tx.Outputs[i].DestinationPublicKey, 0, value, 12, 32);

                    _outputDB.AddValue(key, value);

                    
                }
            }
        }

        public static byte[]? ReadBlock(uint height)
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

        private void CheckGenesisBlock()
        {
            if (ReadBlock(0).Length == 0) // no genesis block
            {

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

        public static ulong RetrieveOutputAmount()
        {
            return 0;
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
