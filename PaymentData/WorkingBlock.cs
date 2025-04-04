﻿using ShakaCoin.Blockchain;
using ShakaCoin.Datastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    public class WorkingBlock: Block
    {

        public byte[] BlockHeader = new byte[117];

        private ulong? _feeSum;

        public MerkleNode? MerkleRootNode;

        public Stopwatch StopWatch = new Stopwatch();

        public WorkingBlock(Block blck)
        {
            BlockHeight = blck.BlockHeight;
            Version = blck.Version;
            TimeStamp = blck.TimeStamp;
            PreviousBlockHash = blck.PreviousBlockHash;
            MerkleRoot = blck.MerkleRoot;
            Target = blck.Target;
            MiningIncrement = blck.MiningIncrement;
            Transactions = blck.Transactions;

        }

        public void OverWriteIncrement()
        {
            byte[] miningIncBytes = BitConverter.GetBytes(MiningIncrement);

            Array.Copy(miningIncBytes, 0, BlockHeader, 109, 8);
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
            MerkleRoot = MerkleRootNode.Hash;

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
                path.Pop();
            }
            

            if (!(mx.Right is null))
            {
                path.Push(mx.Right);
                if (Traverse(mx.Right, target, path))
                {
                    return true;
                }
                path.Pop();
            }
            
            return false;

        }

        public byte[][] GenerateMerkleProof(Transaction tx)
        {
            if (MerkleRootNode is null)
            {
                GenerateMerkleRoot();

            }
            string searchTx = Hasher.GetHexStringQuick(Hasher.Hash256(tx.GetBytes()));

            Stack<MerkleNode> pth = new Stack<MerkleNode>();

            bool res = Traverse(MerkleRootNode, searchTx, pth);

            byte[][] output = new byte[pth.Count][];
            for (int i = 0; i < pth.Count; i++)
            {

                output[i] = pth.ElementAt(pth.Count - 1 - i).Hash;
            }

            List<MerkleNode> siblings = new List<MerkleNode>();

            MerkleNode? Traversal = MerkleRootNode;

            for (int i = 0; i < output.Length; i++)
            {
                if (Hasher.AreTheSame(Traversal.Left.Hash, output[i]))
                {
                    siblings.Add(Traversal.Right);
                    Traversal = Traversal.Left;
                } else
                {
                    siblings.Add(Traversal.Left);
                    Traversal = Traversal.Right;
                }
            }

            byte[][] ox2 = new byte[pth.Count][];
            for (int i = 0; i < output.Length; i++)
            {
                ox2[output.Length - 1 - i] = siblings[i].Hash;
            }


            return ox2;



        }

    }
}
