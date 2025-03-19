using ShakaCoin.Blockchain;
using ShakaCoin.Datastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    public class WorkingBlock: Block
    {

        public byte[] BlockHeader = new byte[117];

        public OutputBloomFilter outputBF;

        private ulong? _feeSum;

        public MerkleNode? MerkleRootNode;

        public WorkingBlock(Block blck)
        {
            BlockHeight = blck.BlockHeight;
            Version = blck.Version;
            TimeStamp = blck.TimeStamp;
            PreviousBlockHash = blck.PreviousBlockHash;
            MerkleRoot = blck.MerkleRoot;
            Target = blck.Target;
            MiningIncrement = blck.MiningIncrement;

            outputBF = new OutputBloomFilter();
        }

        public void OverWriteIncrement()
        {
            byte[] miningIncBytes = BitConverter.GetBytes(MiningIncrement);

            Array.Copy(miningIncBytes, 0, BlockHeader, 109, 8);
        }

        public void SetHeader()
        {

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

        private bool Traverse(MerkleNode mx, string target, Stack<MerkleNode> path)
        {
            if (mx._isLeaf)
            {
                if (Hasher.GetHexStringQuick(mx.Hash) == target)
                {
                    return true;
                }
            }

            if (!(mx.Left is null))
            {
                path.Push(mx.Left);
                if (Traverse(mx.Left, target, path))
                {
                    return true;
                }
            }
            path.Pop();

            if (!(mx.Right is null))
            {
                path.Push(mx.Right);
                if (Traverse(mx.Right, target, path))
                {
                    return true;
                }
            }
            path.Pop();

            return false;

        }

        public byte[] GenerateMerkleProof(Transaction tx)
        {
            string searchTx = Hasher.GetHexStringQuick(Hasher.Hash256(tx.GetBytes()));

            Stack<MerkleNode> pth = new Stack<MerkleNode>();

            bool res = Traverse(MerkleRootNode, searchTx, pth);

            Console.WriteLine(pth.Count);

            return new byte[8];
            
        }

    }
}
