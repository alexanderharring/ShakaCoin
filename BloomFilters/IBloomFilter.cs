using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ShakaCoin.BloomFilters
{
    public interface IBloomFilter
    {
        int Size { get; }
        int HashFunctions { get; }
        BitArray BitArray { get; set; }
        void AddItem(byte[] data);
        bool ProbablyContains(byte[] data);

    }
}
