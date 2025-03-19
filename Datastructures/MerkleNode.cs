using ShakaCoin.Blockchain;
using ShakaCoin.PaymentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Datastructures
{
    public class MerkleNode
    {
        public byte[] Hash;
        public MerkleNode? Left;
        public MerkleNode? Right;
        public bool _isLeaf;
        private bool _isNull;

        public MerkleNode(byte[] hashData)
        {
            Hash = hashData;
            _isLeaf = true;
        }

        public MerkleNode()
        {
            _isNull = true;
            Hash = new byte[32];
        }
        public MerkleNode(MerkleNode _Left, MerkleNode _Right)
        {
            _isLeaf = false;

            byte[] bigArray = new byte[64];

            Buffer.BlockCopy(_Left.Hash, 0, bigArray, 0, 32);
            Buffer.BlockCopy(_Right.Hash, 0, bigArray, 32, 32);

            Hash = Hasher.Hash256(bigArray);

            Left = _Left;
            Right = _Right;
        }

        public void LevelOrderPrint()
        {
            Queue<MerkleNode> q = new Queue<MerkleNode>();
            q.Enqueue(this);

            while (q.Count > 0)
            {
                int n = q.Count;
                for (int i = 0; i < n; i++)
                {
                    MerkleNode node = q.Dequeue();
                    Console.Write(Hasher.GetHexStringQuick(node.Hash) + " ");

                    if (!(node.Left is null))
                    {
                        q.Enqueue(node.Left);
                    }


                    if (!(node.Right is null))
                    {
                        q.Enqueue(node.Right);
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        

    }
}
