using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class GenesisBlock
    {
        internal string pubKey = "3F804A90169864F10F66D7F9D176754F0B1A6EA539443FADA2D759878BA141D1";

        internal GenesisBlock()
        {
            Block gBlock = new Block();

            byte version = 0b10000000; // coinbase transaction

            Transaction gTransaction = new Transaction(version);

            Output ox = new Output()
        }
    }
}
