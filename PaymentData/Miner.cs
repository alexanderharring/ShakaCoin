using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ShakaCoin.Blockchain;
using ShakaCoin.MainInteraction;

namespace ShakaCoin.PaymentData
{
    public class Miner
    {

        private WorkingBlock _candidateBlock;
        private bool _shouldBeMining = false;
        public Block ExtractBlock;

        public Miner()
        {
            _candidateBlock = new WorkingBlock(new Block());
        }

        public async Task StartMining(List<Transaction> txList, byte[] minerPubKey)
        {
            Block prevBlock = Parser.ParseBlock(FileManagement.ReadBlock(FileManagement.Instance.maxBlockNum-1));

            Block tBlock = new Block();
            tBlock.SetTimeStamp();
            tBlock.BlockHeight = FileManagement.Instance.maxBlockNum;
            tBlock.Version = 0x00;

            tBlock.Target = prevBlock.Target;

            //byte[] Target = new byte[32];
            //Target[4] = 0xF0;
            //tBlock.Target = Target;

            tBlock.PreviousBlockHash = prevBlock.GetBlockHash();

            Transaction genesisTransaction = new Transaction(0b10000000);

            Input ix = new Input(new byte[32], 0xFF);

            byte[] fakeSignature = new byte[64];
            byte[] copySource = Hasher.Hash512(Hasher.GetBytesQuick("This is the coinbase transaction of the genesis block. :)" + tBlock.BlockHeight.ToString()));
            Array.Copy(copySource, fakeSignature, copySource.Length);

            ix.AddSignature(fakeSignature);
            genesisTransaction.AddInput(ix);

            Output ox = new Output(256, minerPubKey);

            genesisTransaction.AddOutput(ox);

            tBlock.AddTransaction(genesisTransaction);

            foreach (Transaction tx in txList)
            {
                tBlock.AddTransaction(tx);
            }

            _candidateBlock = new WorkingBlock(tBlock);
            _candidateBlock.GenerateMerkleRoot();

            tBlock.MerkleRoot = _candidateBlock.MerkleRoot;

            Console.WriteLine("Started mining...");
            Console.WriteLine("Press [Enter] to stop mining.");

            _candidateBlock.StopWatch.Start();

            _shouldBeMining = true;

            _ = Task.Run(async () => await MineBlock());

        }

        private async Task MineBlock()
        {
            ulong k = 1000000;
            while (_shouldBeMining)
            {
                if ((_candidateBlock.MiningIncrement % k) == 0)
                {
                    _candidateBlock.StopWatch.Stop();
                    var elapsed = _candidateBlock.StopWatch.Elapsed.TotalSeconds;

                    Console.Write("\rCurrent Increment: " + _candidateBlock.MiningIncrement.ToString() + " Hash Rate: " + Math.Round(k / elapsed, 1).ToString() + " hash/sec");
                    _candidateBlock.StopWatch.Restart();
                }

                _candidateBlock.MiningIncrement++;

                byte[] headerHash = _candidateBlock.GetBlockHash();  

                if (Hasher.IsByteArrayLarger(_candidateBlock.Target, headerHash))
                {
                    Block perfectBlock = (Block)_candidateBlock;
                    perfectBlock.MiningIncrement = _candidateBlock.MiningIncrement;

                    FileManagement.Instance.WriteBlock(perfectBlock);

                    Console.WriteLine("\nMined block!! @ Increment " + _candidateBlock.MiningIncrement.ToString());
                    Console.WriteLine(Hasher.GetHexStringQuick(headerHash));
                    StopMining();
                }
            }
        }



        public void StopMining()
        {
            Console.WriteLine("\nStopped mining...");
            _shouldBeMining = false;
        }
    }
}
