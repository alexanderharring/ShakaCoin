using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.Datastructures
{
    internal interface INodeAVL<T>
    {
        T Value { get; }
        int Height { get; }
        void Insert(T value);
        bool Delete(T value);
        bool Contains(T value);

    }
}
