using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;
using ShakaCoin.PaymentData;

namespace ShakaCoin.Datastructures
{
    public class TXNodeAVL: INodeAVL<Transaction>, IComparable<TXNodeAVL>
    {
        
        public int Height { get; set; }
        public Transaction Value { get; set; }
        public TXNodeAVL? Left;
        public TXNodeAVL? Right;

        public TXNodeAVL(Transaction val)
        {
            Value = val;
        }

        public void Insert(Transaction val)
        {
            this.Insert(new TXNodeAVL(val));
        }

        public bool Delete(Transaction val)
        {
            return Remove(new TXNodeAVL(val), out Left);
        }

        public bool Contains(Transaction val)
        {
            return Contains(new TXNodeAVL(val));
        }

        public bool Contains(TXNodeAVL val)
        {
            if (val < this)
            {
                return (Left?.Contains(val) ?? false);
            } else if (val > this)
            {
                return (Right?.Contains(val) ?? false);
            } else if (val == this)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public int CompareTo(TXNodeAVL? other)
        {
            if (other is null)
            {
                return 1;
            }

            double f1 = Value.CalculateFeeRate();
            double f2 = other.Value.CalculateFeeRate();

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

        private void Insert(TXNodeAVL insertNode)
        {
            if (insertNode < this)
            {
                if (!(Left is null))
                {
                    Left.Insert(insertNode);
                } else
                {
                    Left = insertNode;
                }

                
            } else //putting equal values right instead of doing some sort of like duplicate counter or linked list thingy
            {
                if (!(Right is null))
                {
                    Right.Insert(insertNode);
                }
                else
                {
                    
                    Right = insertNode;
                }
            }

            UpdateHeight();

            Balance();
        }

        private void Swap(TXNodeAVL other)
        {
            Transaction tempTX = other.Value;

            other.Value = this.Value;
            this.Value = tempTX;
        }

        private void RightRight()
        {
            if (Left is null)
            {
                throw new InvalidOperationException("no left :(");
            }


            this.Swap(Left);

            TXNodeAVL? oldRight = Right;
            Right = Left;
            Left = Right.Left;
            Right.Left = Right.Right;
            Right.Right = oldRight;

            Right.UpdateHeight();
            UpdateHeight();
        }

        private void LeftLeft()
        {
            if (Right is null)
            {
                throw new InvalidOperationException("no right :(");
            }


            this.Swap(Right);

            TXNodeAVL? oldLeft = Left;
            Left = Right;
            Right = Left.Right;
            Left.Right = Left.Left;
            Left.Left = oldLeft;

            Left.UpdateHeight();
            UpdateHeight();
        }

        private void UpdateHeight()
        {
            Height = 1 + Math.Max(Left?.Height ?? -1, Right?.Height ?? -1);
        }

        private int GetBalance()
        {
            return (Right?.Height ?? -1 )-(Left?.Height ?? -1);
        }

        private void Balance()
        {
            int bf = GetBalance();

            if (bf < -1)
            {

                if (Left?.GetBalance() == 1)
                {
                    Left.LeftLeft();
                }

                RightRight();

            } 
            else if (bf > 1)
            {
                if (Right?.GetBalance() == -1)
                {
                    Right.RightRight();
                }
                LeftLeft();
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

        public override bool Equals(Object? obj)
        {
            var item = obj as TXNodeAVL;
            if (item is null)
            {
                return false;
            }
            else
            {
                return this.Value == item.Value;
            }
            
        }

        private TXNodeAVL Max()
        {
            if (Right is null)
            {
                return this;
            } else
            {
                return Right.Max();
            }
        }

        public bool Remove(TXNodeAVL node, out TXNodeAVL? _root)
        {
            bool didRemove;

            if (node < this)
            {
                _root = this;
                didRemove = Left?.Remove(node, out Left) ?? false;
            }
            else if (node > this)
            {
                _root = this;
                didRemove = Right?.Remove(node, out Right) ?? false;
            }

            else if ((Left is null) || (Right is null))
            {
                _root = Left ?? Right;
                didRemove = true;
            } else
            {
                TXNodeAVL lMax = Left.Max();

                Left.Remove(new TXNodeAVL(lMax.Value), out Left);
                Value = lMax.Value;

                _root = this;
                didRemove = true;
            }

            _root?.UpdateHeight();
            _root?.Balance();

            return didRemove;
        }

        public override int GetHashCode()
        {
            return OutputBloomFilter.GetHashIndexStatic(Value.GetBytes(), int.MaxValue);
        }

        public List<Transaction> GetNTransactions(int N)
        {
            List<Transaction> txList = new List<Transaction>();
            this.TraulTransactionsReverse(txList, N);

            return txList;

        }

        public List<Transaction> GetAllTransactions()
        {
            List<Transaction> txl = new List<Transaction>();
            this.TraulAllTx(txl);

            return txl;
        }

        private void TraulAllTx(List<Transaction> txL)
        {
            if (!(Right is null))
            {
                Right.ReverseInOrder();
            }

            txL.Add(Value);

            if (!(Left is null))
            {
                Left.ReverseInOrder();
            }
        }
        private void TraulTransactionsReverse(List<Transaction> txList, int maxN)
        {
            if (txList.Count >= maxN)
            {
                return;
            }

            if (!(Right is null))
            {
                Right.TraulTransactionsReverse(txList, maxN);
            }

            if (txList.Count < maxN)
            {
                txList.Add(Value);
            } else
            {
                return;
            }



            if (!(Left is null))
            {
                Left.TraulTransactionsReverse(txList, maxN);
            }

        }

        public void ReverseInOrder()
        {
            if (!(Right is null))
            {
                Right.ReverseInOrder();
            }
            
            Console.WriteLine(Math.Round(Value.CalculateFeeRate(), 1));

            if (!(Left is null))
            {
                Left.ReverseInOrder();
            }
        }

        private int GetMaxWidth()
        {
            return 1 + Math.Max(Left?.GetMaxWidth() ?? 0, Right?.GetMaxWidth() ?? 0);
        }

        public void PrintTree(string indent, bool isRight)
        {
             
            Console.Write(indent);

            if (isRight)
            {
                Console.Write("R--");
                indent += "   ";

            } else

            {
                Console.Write("L--");
                indent += "|  ";
            }
            Console.WriteLine(Math.Round(this.Value.CalculateFeeRate(), 1));

            if (!(Right is null))
            {
                Right.PrintTree(indent, true);
            }

            if (!(Left is null)){

                Left.PrintTree(indent, false);
            }
        }

        public void LevelOrderTraversal()
        {
            List<(TXNodeAVL, int)> ls = new List<(TXNodeAVL, int)>();

            ls.Add((this, 0));

            int mw = this.GetMaxWidth();
            int level = 0;

            while (ls.Count > 0)
            {
                int levelS = ls.Count;

                string[] lp = Enumerable.Repeat(" ", (int)Math.Pow(2, this.Height)).ToArray();

                for (int t = 0; t < levelS; t++)
                {
                    (TXNodeAVL, int) v = ls[0];
                    ls.RemoveAt(0);


                    lp[v.Item2] = Math.Round(v.Item1.Value.CalculateFeeRate(), 1).ToString();

                     
                    if (!(v.Item1.Left is null))
                    {
                        ls.Add((v.Item1.Left, 2*v.Item2));
                    }

                    if (!(v.Item1.Right is null))
                    {
                        ls.Add((v.Item1.Right, 2 * v.Item2 + 1));
                    }


                }

                level++;
                Console.WriteLine(String.Join(" ", lp));
                Console.WriteLine("");

            }
        }

    }
}
