using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Transaction
    {

        public byte Version;

        public List<Input> Inputs = new List<Input>();

        public List<Output> Outputs = new List<Output>();

        private ulong? _Fee;

        private double? _FeeRate;

        public Transaction(byte version) 
        {
            Version = version;
        }

        public void AddInput(Input newInput)
        {
            Inputs.Add(newInput);
        }

        public void AddOutput(Output newOutput)
        {
            Outputs.Add(newOutput);
        }

        public double CalculateFeeRate()
        {
            if (_FeeRate != null)
            {
                return (double)_FeeRate;
            } else
            {
                CalculateFee();
                return (double)_FeeRate;
            }
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
                feeSum += outpt.Amount;
            }

            _Fee = feeSum;

            _FeeRate = (double)feeSum / GetBytes().Length;

            return (ulong)_Fee;
        }

        public byte[] GetBytes()
        {
            // version (1) + inputcount (1) + inputs( n * 97 ) + outputcount (1) + outputs ( n * 40 )
            byte[] TransactionBytes = new byte[3 + 97 * Inputs.Count + 40 * Outputs.Count];

            TransactionBytes[0] = Version;
            TransactionBytes[1] = (byte)Inputs.Count;

            for (int i = 0; i < Inputs.Count; i++)
            {
                int startingInd = 2 + i * 97;
                Buffer.BlockCopy(Inputs[i].GetBytes(), 0, TransactionBytes, 0, 97);
            }

            TransactionBytes[2 + 97 * Inputs.Count] = (byte)Outputs.Count;

            for (int i = 0; i < Outputs.Count; i++)
            {
                int startingInd = 3 + 97 * Inputs.Count + i * 40;
                Buffer.BlockCopy(Outputs[i].ExportToBytes(), 0, TransactionBytes, 0, 40);
            }

            return TransactionBytes;

        }


        public bool IsCoinbase()
        {
            return ((Version & 128) != 0);
        }

    }
}
