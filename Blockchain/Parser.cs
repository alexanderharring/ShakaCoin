using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.PaymentData;

namespace ShakaCoin.Blockchain
{
    public class Parser
    {
        public static Output ParseOutput(byte[] data)
        {
            if (data.Length != 40)
            {
                throw new ArgumentException();
            }

            ulong amount = BitConverter.ToUInt64(data, 32);

            Console.WriteLine(amount);
            byte[] pk = new byte[32];
            Buffer.BlockCopy(data, 0, pk, 0, 32);

            return new Output(amount, pk);
        }

        public static Input ParseInput(byte[] data)
        {
            if (data.Length != 97)
            {
                
                throw new ArgumentException();
            }


            byte[] txid = new byte[32];
            Buffer.BlockCopy(data, 0, txid, 0, 32);
            byte oind = data[32];
            byte[] sig = new byte[64];
            Buffer.BlockCopy(data, 33, sig, 0, 64);

            Input ix = new Input(txid, oind);
            ix.AddSignature(sig);

            return ix;
        }

        public static Transaction ParseTransaction(byte[] data)
        {
            byte version = data[0];
            byte icount = data[1];

            Transaction buildTX = new Transaction(version);

            for (int i = 0; i < icount; i++)
            {
                byte[] ixBytes = new byte[97];

                Buffer.BlockCopy(data, 2 + i * 97, ixBytes, 0, 97);

                Input ix = ParseInput(data);

                buildTX.AddInput(ix);
            }

            byte ocount = data[2 + 97 * icount];

            for (int i = 0; i < ocount; i++)
            {
                byte[] oxBytes = new byte[40];
                
                Buffer.BlockCopy(data, 3 + 97 * icount + i * 40, oxBytes, 0, 40);

                Output ox = ParseOutput(oxBytes);

                buildTX.AddOutput(ox);
            }

            return buildTX;
        }

        //    public static Block ParseBlock(byte[] data)
        //    {

        //        byte[] bHeader = new byte[117];

        //        Buffer.BlockCopy(data, 0, bHeader, 0, 117);

        //        uint blockHeight = BitConverter.ToUInt32(bHeader, 0);

        //        Block buildBlock = new Block();

        //        //buildBlock.TimeStamp; // change the whole block thingy to like a Block class and then a working block class that inherits it to work on maybe?

        //        ushort txCount = BitConverter.ToUInt16(data, 117);

        //        int offset = 0;

        //        for (int i = 0; i < txCount; i++)
        //        {
        //            ushort txSize = BitConverter.ToUInt16(data, 119 + offset);
        //            byte[] txBytes = new byte[txSize];

        //            Buffer.BlockCopy(data, offset+2, txBytes, 0, txSize);

        //            Transaction tx = ParseTransaction(txBytes);

        //            offset += (txSize+2);

        //            buildBlock.AddTransaction(tx);
        //        }
        //    }

    }
}
