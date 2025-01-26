using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class Transaction
    {

        public byte Version;

        public ushort InputsCount;

        public ushort OutputsCount;

        public List<Input> Inputs = new List<Input>();

        public List<Output> Outputs = new List<Output>();
    }
}
