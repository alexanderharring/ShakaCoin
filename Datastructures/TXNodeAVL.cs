using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.PaymentData;

namespace ShakaCoin.Datastructures
{
    internal class TXNodeAVL: INodeAVL<Transaction>, IComparable<TXNodeAVL>
    {
        
        public int Height { get; set; }
        public Transaction Value { get; set; }
        public TXNodeAVL? Left {  get; set; }
        public TXNodeAVL? Right {  get; set; }

        public TXNodeAVL(Transaction val)
        {
            Value = val;
        }

        public void Insert(Transaction val)
        {

        }

        public void Delete(Transaction val)
        {

        }

        public bool Contains(Transaction val)
        {
            return false;
        }

        public int CompareTo(TXNodeAVL? other)
        {
            if (other == null)
            {
                return 1;
            }

            ulong f1 = Value.CalculateFee();
            ulong f2 = other.Value.CalculateFee();

            if (f1 == f2)
            {
                return 0;
            }

            if (f1 > f2)
            {
                return 1;
            }

            return -1;
        }
    }
}
