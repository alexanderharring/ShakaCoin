using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace ShakaCoin.Cryptography
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
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        public static string GetStringQuick(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public static string GetHexStringQuick(byte[] data)
        {
            return Convert.ToHexString(data);
        }
        
        public static byte[] GetBytesFromHexStringQuick(string data)
        {
            return Convert.FromHexString(data);
        }
 
        
    }
}
