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

        public List<Output> Outputs = new List<Output>();

        private ulong? _Fee;

        public Transaction(byte version, ushort icount) 
        {
            Version = version;
            InputsCount = icount;
        }

        public void AddInput(Input newInput)
        {
            Inputs.Add(newInput);
        }

        public void AddOutput(Output newOutput)
        {
            Outputs.Add(newOutput);
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

            foreach (Output outpt in Outputs)
            {
                feeSum -= outpt.Amount;
            }

            _Fee = feeSum;

            return (ulong)_Fee;
        }


    }
}
