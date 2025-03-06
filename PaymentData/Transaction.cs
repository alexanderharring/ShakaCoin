using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Transaction
    {

        public byte Version;

        public ushort InputsCount;

        public List<Input> Inputs = new List<Input>();

        public bool IsReturning;

        public Output? MainOutput;

        public Output? ReturnOutput;

        private ulong? _Fee;

        internal Transaction(byte version, ushort icount, bool isReturning)
        {
            Version = version;
            InputsCount = icount;
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
            if (_Fee != null)
            {
                return (ulong)_Fee;
            }

            ulong feeSum = 0;

            foreach (Input input in Inputs)
            {
                feeSum += FileManagement.RetrieveOutputAmount();
            }

            foreach (Output outpt in GetOutputs())
            {
                feeSum -= outpt.Amount;
            }

            _Fee = feeSum;

            return (ulong)_Fee;
        }


    }
}
