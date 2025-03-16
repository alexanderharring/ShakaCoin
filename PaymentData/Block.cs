using ShakaCoin.PaymentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ShakaCoin.Datastructures;

namespace ShakaCoin.PaymentData
{
    public class Block
    {
        public byte[] PreviousBlockHash;

        public byte[] MerkleRoot = new byte[32];

        public byte Version;

        public uint BlockHeight;

        public long TimeStamp;

        public uint MiningIncrement = 0;

        public byte[] Target = new byte[32];

        public ushort TransactionCount;

        private TXNodeAVL _TXroot;

        public byte[] BlockHeader = new byte[209];

        public OutputBloomFilter outputBF;

        private ulong? _feeSum;

        public Block()
        {
            outputBF = new OutputBloomFilter();
            
        }

        public void AddTransaction(Transaction newTx)
        {
            _TXroot.Insert(newTx);

            if (!(newTx.IsCoinbase())) {

                foreach (Output ox in newTx.Outputs)
                {
                    outputBF.AddItem(ox.ExportToBytes());
                }
            }


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

        public ulong GetBlockFee()
        {
            if (_feeSum != null)
            {
                return (ulong)_feeSum;
            }

            _feeSum = 0;

            foreach (Transaction tx in Transactions)
            {
                _feeSum += tx.CalculateFee();
            }

            return (ulong)_feeSum;
        }

        public byte[] GetBytes()
        {
            return [];
        }



    }
}
