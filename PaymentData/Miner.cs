using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;

namespace ShakaCoin.PaymentData
{
    public class Miner
    {

        private bool _isMining = false;

        private Block _candidateBlock;

        public Miner()
        {
            _candidateBlock = new Block();
        }
    }
}
