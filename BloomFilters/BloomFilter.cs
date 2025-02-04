using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using ShakaCoin.Cryptography;

namespace ShakaCoin.BloomFilters
{
    internal class BloomFilter
    {
        public required BitArray bitArray;

        internal BloomFilter(int size)
        {
            BitArray bitArray = new BitArray(size); //82 items gives a size of about 512 bits
        }

        internal int GetHash(byte[] data)
        {
            var newData = Hasher.Hash512(data);

            return 0;
        }

    }
}
