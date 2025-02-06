using Org.BouncyCastle.Bcpg;
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

        public Output? MainOutput;

        public Output? ReturnOutput;

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

        public List<Output> GetOutputs()
        {
            List<Output> outputs = new List<Output>();
            if (MainOutput != null)
            {
                outputs.Add(MainOutput);
            }
            
            if (IsReturning && (ReturnOutput != null))
            {
                outputs.Add(ReturnOutput);
            }

            return outputs;
        }

        public ulong CalculateFee()
        {
            ulong feeSum = 0;

            foreach (Input input in Inputs)
            {
                feeSum += 
            }
        }
    }
}
