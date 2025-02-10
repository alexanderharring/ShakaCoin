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

        public int CalcualteBF()
        {

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

        private TXNodeAVL Insert(TXNodeAVL insertNode)
        {
            if (insertNode < this)
            {
                if (Left != null)
                {
                    Left.Insert(insertNode);
                } else
                {
                    Left = insertNode;
                }

                
            } else //putting equal values right instead of doing some sort of like duplicate counter or linked list thingy
            {
                if (Right != null)
                {
                    Right.Insert(insertNode);
                }
                else
                {
                    Right = insertNode;
                }
            }

            
        }

        public static bool operator >(TXNodeAVL a1, TXNodeAVL a2)
        {
            return a1.CompareTo(a2) > 0;
        }

        public static bool operator <(TXNodeAVL a1, TXNodeAVL a2)
        {
            return a1.CompareTo(a2) < 0;
        }

        public static bool operator ==(TXNodeAVL a1, TXNodeAVL a2)
        {
            return a1.CompareTo(a2) == 0;
        }

        public static bool operator !=(TXNodeAVL a1, TXNodeAVL a2)
        {
            return a1.CompareTo(a2) != 0;
        }
    }
}
