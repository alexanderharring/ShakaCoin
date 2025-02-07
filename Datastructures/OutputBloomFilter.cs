using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ShakaCoin.Cryptography;

namespace ShakaCoin.Datastructures
{
    public class OutputBloomFilter : IBloomFilter
    {
        public int Size => 512;
        public int HashFunctions => 4;
        public BitArray BitArray { get; set; }

        public OutputBloomFilter()
        {
            BitArray = new BitArray(512);
        }

        public ushort GetHashIndex(byte[] data)
        {
            byte[] hashed = Hasher.Hash256(data);

            ushort lastTwo = BitConverter.ToUInt16(hashed, hashed.Length - 2);

            ushort extract = (ushort)(lastTwo & 0x1FF);

            return extract;
            
        }


        public void AddItem(byte[] data)
        {
            int mainLength = data.Length;

            byte[] newData = new byte[mainLength + 1];
            Array.Copy(data, newData, mainLength);
 
            for (int i=1; i < 5; i++)
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

            for (int i = 1; i < 5; i++)
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

    }
}
