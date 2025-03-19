using ShakaCoin.PaymentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ShakaCoin.Datastructures;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Block
    {
        public byte[] PreviousBlockHash = new byte[32];

        public byte[] MerkleRoot = new byte[32];

        public byte Version;

        public uint BlockHeight;

        public long TimeStamp;

        public ulong MiningIncrement = 0;

        public byte[] Target = new byte[32];

        public List<Transaction> Transactions = new List<Transaction>();

        private ulong? _feeSum;

        public int? BlockSize;

        public Block()
        {

            //BlockHeight = bHeight;
            
        }

        public void AddTransaction(Transaction newTx)
        {
            Transactions.Add(newTx);

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

        public byte[] GetBlockHeader()
        {
            byte[] headerBuild = new byte[117];

            byte[] heightBytes = BitConverter.GetBytes(BlockHeight);

            Array.Copy(heightBytes, 0, headerBuild, 0, 4);

            heightBytes[4] = Version;

            byte[] timeBytes = BitConverter.GetBytes(TimeStamp);
            Array.Copy(timeBytes, 0, headerBuild, 5, 8);

            Array.Copy(PreviousBlockHash, 0, headerBuild, 13, 32);
            Array.Copy(MerkleRoot, 0, headerBuild, 45, 32);
            Array.Copy(Target, 0, headerBuild, 77, 32);

            byte[] miningIncBytes = BitConverter.GetBytes(MiningIncrement);
            Array.Copy(Target, 0, headerBuild, 109, 8);

            return headerBuild;
        }
        
        public byte[] GetBlockBytes()
        {
            if (BlockSize is null)
            {
                BlockSize = 119;
                foreach (Transaction tx in Transactions)
                {
                    BlockSize += tx.GetBytes().Length;
                }

            }
            byte[] blockBytes = new byte[(int)BlockSize];

            byte[] blockHeader = GetBlockHeader();

            Buffer.BlockCopy(blockHeader, 0, blockBytes, 0, 117);

            ushort TransactionCount = (ushort)Transactions.Count;
            Buffer.BlockCopy(BitConverter.GetBytes(TransactionCount), 0, blockBytes, 117, 2);

            int offset = 0;

            for (int i = 0; i < TransactionCount; i++)
            {
                byte[] thisTx = Transactions[i].GetBytes();
                ushort TxSize = (ushort)thisTx.Length; // the maximum size of a transaction is about 34kB which fits inside of a ushort
                Buffer.BlockCopy(BitConverter.GetBytes(TxSize), 0, blockBytes, 119 + offset, 2);
                Buffer.BlockCopy(thisTx, 0, blockBytes, 121 + offset, thisTx.Length);
                offset += (thisTx.Length + 2);
            }

            return blockBytes;
        }

        

        public byte[] GetBlockHash()
        {
            return Hasher.Hash256(GetBlockHeader());
        }

    }
}
