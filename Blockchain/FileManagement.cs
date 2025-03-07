using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.PaymentData;

namespace ShakaCoin.Blockchain
{
    public class FileManagement
    {
        private static string _appName = "ShakaCoin";
        private static string _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);
        private static string BlockDir = Path.Combine(_appDataPath, "Blocks");
        private static string OutputsDir = Path.Combine(_appDataPath, "Outputs");


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
        }

        public void WriteBlock(Block block)
        {
            string fName = "b" + block.BlockHeight.ToString() + ".dat";
            string fileP = Path.Combine(BlockDir, fName);

            File.WriteAllBytes(fileP, block.GetBytes());
        }

        public void TestFile()
        {
            string filep = Path.Combine(_appDataPath, "test.txt");
            File.WriteAllText(filep, "TestData");

        }

        public static ulong RetrieveOutputAmount()
        {
            return 0;
        }
    }
}
