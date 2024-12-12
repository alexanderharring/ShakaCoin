using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ShakaCoin.PaymentData
{
    internal class Block
    {
        public string BlockHash;
        public string PreviousBlockHash;

        public List<Transaction> Transactions;

        public Block()
        {
            
        }

    }
}
