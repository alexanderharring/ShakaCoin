using ShakaCoin.Datastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    public class TransactionPool
    {
        private TXNodeAVL? _TXroot;

        private int txPoolSize = 0;

        private int maxTXSize = 200_000;
        public TransactionPool()
        {
            //_TXroot = new TXNodeAVL()


        }

        public List<Transaction> GetCandidateTransactions()
        {
            if (_TXroot is null)
            {
                return new List<Transaction>();
            } else
            {
                List<Transaction> txn = _TXroot.GetNBytesOfTransactions(maxTXSize - txPoolSize);

                foreach (Transaction t in txn)
                {
                    _TXroot.Delete(t);
                    txPoolSize -= t.GetBytes().Length;
                }

                return txn;
            }
        }

        public void AddTx(Transaction nTX)
        {
            txPoolSize += nTX.GetBytes().Length;

            if (!(_TXroot is null))
            {
                _TXroot.Insert(nTX);
            } else
            {
                _TXroot = new TXNodeAVL(nTX);
            }

            
        }
    }
}
