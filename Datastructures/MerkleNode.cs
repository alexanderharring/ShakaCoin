using ShakaCoin.Cryptography;
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
        public byte[] Hash { get; set; }
        public MerkleNode? Left { get; set; }
        public MerkleNode? Right { get; set; }

        public MerkleNode(byte[] hashData)
        {
            Hash = hashData;
        }

        public MerkleNode(MerkleNode Left, MerkleNode Right)
        {

            byte[] bigArray = new byte[128];

            Buffer.BlockCopy(Left.Hash, 0, bigArray, 0, 64);
            Buffer.BlockCopy(Right.Hash, 0, bigArray, 64, 64);

            Hash = Hasher.Hash512(bigArray);
        }
    }
}
