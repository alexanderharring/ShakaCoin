using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ShakaCoin.Blockchain;

namespace ShakaCoin.Datastructures
{
    public class OutputBloomFilter : IBloomFilter
    {
        public int Size => 1024;
        public int HashFunctions => 3;
        public BitArray BitArray { get; set; }

        public OutputBloomFilter()
        {
            BitArray = new BitArray(Size);
        }

        public static int GetHashIndexStatic(byte[] data, int Size)
        {
            byte[] hashed = Hasher.Hash256(data);

            ushort lastTwo = BitConverter.ToUInt16(hashed, hashed.Length - 2);

            int extract = (lastTwo & (Size - 1));

            return extract;
        }

        public int GetHashIndex(byte[] data)
        {
            return GetHashIndexStatic(data, Size);
   
        }


        public void AddItem(byte[] data)
        {
            int mainLength = data.Length;

            byte[] newData = new byte[mainLength + 1];
            Array.Copy(data, newData, mainLength);
 
            for (int i=1; i < (1+HashFunctions); i++)
            {
                newData[mainLength] = (byte)i;
                BitArray[GetHashIndex(newData)] = true;
            }
        }

        public void AddItem(string data)
        {
            AddItem(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public bool ProbablyContains(byte[] data)
        {

            int mainLength = data.Length;

            byte[] newData = new byte[mainLength + 1];
            Array.Copy(data, newData, mainLength);

            for (int i = 1; i < (1 + HashFunctions); i++)
            {
                newData[mainLength] = (byte)i;
                if (!BitArray[GetHashIndex(newData)]){
                    return false;
                }
            }

            return true;
        }

        public bool ProbablyContains(string data)
        {
            return ProbablyContains(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public byte[] GetBytes()
        {
            byte[] m = new byte[Size/8];
            BitArray.CopyTo(m, 0);

            return m;
        }

    }
}
