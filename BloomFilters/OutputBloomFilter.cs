using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ShakaCoin.BloomFilters
{
    public class OutputBloomFilter : IBloomFilter
    {
        public int Size => 512;
        public int HashFunctions => 4;
        private BitArray BitArray => new BitArray(512);

        public OutputBloomFilter()
        {

        }

        private int Hash(byte[] data, int seed)
        {


            return 0;
        }

        public void AddItem(byte[] data)
        {

        }

        public bool ProbablyContains(byte[] data)
        {
            return false;
        }

    }
}
