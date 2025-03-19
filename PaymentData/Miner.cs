using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Miner
    {

        private WorkingBlock _candidateBlock;
        private CancellationTokenSource _miningCancel = new();

        public Miner()
        {
            _candidateBlock = new WorkingBlock(new Block());
        }

        public async Task StartMining()
        {
            Console.WriteLine("Started mining...");

            while (!(_miningCancel.IsCancellationRequested))
            {
                MineBlock();
                await Task.Delay(150);
            }
        }

        private void MineBlock()
        {
            while (!(_miningCancel.IsCancellationRequested))
            {
                _candidateBlock.MiningIncrement++;
                _candidateBlock.OverWriteIncrement();

                byte[] headerHash = _candidateBlock.GetBlockHash();  

                if (ShouldAcceptHash(headerHash, _candidateBlock.Target))
                {
                    Console.WriteLine("Mined block!!");
                    StopMining();
                }
            }
        }

        private bool ShouldAcceptHash(byte[] hdrHash, byte[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (hdrHash[i] < target[i])
                {
                    return false;
                }
                if (hdrHash[i] > target[i])
                {
                    return true;
                }
            }
            return true;
        } 

        public void StopMining()
        {
            Console.WriteLine("Stopped mining...");
            _miningCancel.Cancel();
        }
    }
}
