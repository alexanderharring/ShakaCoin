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

        public ushort TransactionCount;

        public List<Transaction> Transactions = new List<Transaction>();

        public byte[] BlockHeader = new byte[117];

        public OutputBloomFilter outputBF;

        private ulong? _feeSum;

        public MerkleNode? MerkleRootNode;

        public int? BlockSize;

        public Block(uint bHeight)
        {
            outputBF = new OutputBloomFilter();

            BlockHeight = bHeight;
            
        }

        public void AddTransaction(Transaction newTx)
        {
            Transactions.Add(newTx);

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

            Array.Copy(miningIncBytes, 0, BlockHeader, 109, 8);
        }

        public void SetHeader() {

            // order -> height (4) + version (1) + timestamp (8) + prev hash (32) + merkle root (32) + target ( 32 ) + mining increment (8)
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

            SetHeader();
            Buffer.BlockCopy(BlockHeader, 0, blockBytes, 0, 117);

            TransactionCount = (ushort)Transactions.Count;
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

        public void GenerateMerkleRoot()
        {
            List<Transaction> merkleList = new List<Transaction>(Transactions);

            Queue<MerkleNode> txQueue = new Queue<MerkleNode>();

            foreach (Transaction tx in merkleList)
            {
                txQueue.Enqueue(new MerkleNode(Hasher.Hash256(tx.GetBytes())));
            }

            

            while (txQueue.Count > 1)
            {
                MerkleNode mNode = txQueue.Dequeue();
                MerkleNode? mNode0;
                if (txQueue.Count == 0)
                {
                    mNode0 = new MerkleNode();
                }
                else
                {
                     mNode0 = txQueue.Dequeue();
                }

                txQueue.Enqueue(new MerkleNode(mNode, mNode0));

            }

            MerkleRootNode = txQueue.Dequeue();

        }

        public byte[] GetBlockHash()
        {
            return Hasher.Hash256(BlockHeader);
        }

    }
}
