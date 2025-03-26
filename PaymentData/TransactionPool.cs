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
                List<Transaction> txn = _TXroot.GetNTransactions(200);

                List<Transaction> acceptedTX = new List<Transaction>();

                int sizeSum = 0;

                foreach (Transaction t in txn)
                {
                    sizeSum += t.GetBytes().Length;

                    if (sizeSum < maxTXSize)
                    {
                        acceptedTX.Add(t);
                    } else
                    {
                        break;
                    }
                }

                foreach (Transaction t in acceptedTX)
                {
                    _TXroot.Delete(t);
                    txPoolSize -= t.GetBytes().Length;
                }

                return acceptedTX;
            }
        }

        public int GetTxPoolSize()
        {
            return txPoolSize;
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
