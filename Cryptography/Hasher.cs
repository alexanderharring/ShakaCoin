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
            using (SHA512 sha = new SHA512Managed())
            {
                return sha.ComputeHash(data);
            }
        }

        public static byte[] Hash256(byte[] data)
        {
            using (SHA256 sha = new SHA256Managed())
            {
                return sha.ComputeHash(data);
            }
        }
 
        
    }
}
