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

        public bool IsReturning;

        public Output MainOutput = new Output();

        public Output ReturnOutput = new Output();

        internal Transaction(byte version, ushort icount, ushort ocount, bool isReturning)
        {
            Version = version;
            InputsCount = icount;
            OutputsCount = ocount;
            IsReturning = isReturning;
        }

        internal void AddInput(Input newInput)
        {
            Inputs.Add(newInput);
        }

        internal void SetOutput(Output newOutput)
        {
            MainOutput = newOutput;
        }

        internal void SetReturnOutput(Output returnOutput)
        {
            ReturnOutput = returnOutput;
        }

    }
}
