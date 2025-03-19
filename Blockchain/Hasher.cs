using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace ShakaCoin.Blockchain
{
    public class Hasher
    {
        public static byte[] Hash512(byte[] data)
        {
            using (SHA512 sha = SHA512.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        public static byte[] Hash256(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        public static byte[] GetBytesQuick(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetStringQuick(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static string GetHexStringQuick(byte[] data)
        {
            return Convert.ToHexString(data);
        }

        public static byte[] GetBytesFromHexStringQuick(string data)
        {
            try
            {
                return Convert.FromHexString(data);
            } catch
            {
                return [];
            }
        }

        public static bool IsByteArrayLarger(byte[] data0, byte[] data1)
        {
            for (int i = 0; i < data0.Length; i++)
            {
                if (data0[i] < data1[i])
                {
                    return false;
                }
                if (data0[i] > data1[i])
                {
                    return true;
                }
            }
            return true;
        }

        public static byte[] GetSmallerByteArray(byte[] data0, byte[] data1)
        {
            if (IsByteArrayLarger(data0, data1))
            {
                return data1;
            }
            else
            {
                return data0;
            }
        }

        public static byte[] GetLargerByteArray(byte[] data0, byte[] data1)
        {
            if (IsByteArrayLarger(data0, data1))
            {
                return data0;
            }
            else
            {
                return data1;
            }
        }

        public static bool AreTheSame(byte[] data1, byte[] data2)
        {
            if (data1 is null)
            {
                return false;
            }

            if (data2 is null)
            {
                return false;
            }

            return GetHexStringQuick(data1) == GetHexStringQuick(data2);
        }


    }
}
