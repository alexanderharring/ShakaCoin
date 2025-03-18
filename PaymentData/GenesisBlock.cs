using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class GenesisBlock
    {

        public static Block MakeGenesisBlock()
        {
            Block gBlock = new Block(0);

            string pubKey = "3F804A90169864F10F66D7F9D176754F0B1A6EA539443FADA2D759878BA141D1";

            byte version = 0b10000000; // coinbase transaction

            ulong initialBR = (ulong)256;

            Transaction gTransaction = new Transaction(version);

            Output ox = new Output(initialBR, Hasher.GetBytesFromHexStringQuick(pubKey));

            Input ix = new Input(new byte[32], 0xFF);

            byte[] fakeSignature = new byte[64];

            byte[] copySource = Hasher.GetBytesQuick("This is the coinbase transaction of the genesis block.");
            Array.Copy(copySource, fakeSignature, copySource.Length);

            ix.AddSignature(fakeSignature);

            gTransaction.AddInput(ix);
            gTransaction.AddOutput(ox);

            gBlock.AddTransaction(gTransaction);


            return gBlock;
        }
    }
}
