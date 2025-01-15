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
        public string PreviousBlockHash;

        public long TimeStamp;
        public long Cycle;
        public int TransactionNum;

        public List<Transaction> Transactions;

        public Block()
        {
            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            Console.WriteLine(TimeStamp);
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
        }

        public 

    }
}
