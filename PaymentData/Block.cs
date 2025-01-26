using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ShakaCoin.PaymentData
{
    internal class Block
    {
        public byte[] PreviousBlockHash = new byte[64];

        public byte[] MerkleRoot = new byte[64];

        public byte Version;

        public uint BlockHeight;

        public long TimeStamp;

        public uint MiningIncrement = 0;

        public byte[] Target = new byte[64];

        public ushort TransactionCount;

        public List<Transaction> Transactions = new List<Transaction>();

        public byte[] BlockHeader = new byte[209];

        public Block()
        {
            
        }

        public void OverWriteIncrement()
        {
            byte[] miningIncBytes = BitConverter.GetBytes(MiningIncrement);

            Array.Copy(Target, 0, BlockHeader, 205, 4);
        }

        public void SetHeader() {

            // order -> height (4) + version (1) + timestamp (8) + prev hash (64) + merkle root (64) + target ( 64 ) + mining increment (4)
            byte[] headerBuild = new byte[209]; //209 is the sum of the prev block hash, merkle root .. target

            byte[] heightBytes = BitConverter.GetBytes(BlockHeight);

            Array.Copy(heightBytes, 0, headerBuild, 0, 4);

            heightBytes[4] = Version;

            byte[] timeBytes = BitConverter.GetBytes(TimeStamp);
            Array.Copy(timeBytes, 0, headerBuild, 5, 8);

            Array.Copy(PreviousBlockHash, 0, headerBuild, 13, 64);
            Array.Copy(MerkleRoot, 0, headerBuild, 77, 64);
            Array.Copy(Target, 0, headerBuild, 141, 64);

            byte[] miningIncBytes = BitConverter.GetBytes(MiningIncrement);
            Array.Copy(Target, 0, headerBuild, 205, 4);

            BlockHeader = headerBuild;
        }


    }
}
