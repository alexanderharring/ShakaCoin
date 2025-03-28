using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakaCoin.Blockchain;
using ShakaCoin.Datastructures;
using ShakaCoin.Networking;

namespace ShakaCoin.PaymentData
{
    internal class BlockchainHandler
    {
        private bool _isValidator;
        private TransactionPool _txPool;
        private Miner? _miner;
        private FileManagement _fm;
        private uint _currentBlockHeight = 0;
        private bool _isMining = false;

        public PeerManager? _peerManager;
        public byte[] MinerPubKey = new byte[32];

        private HashSet<uint> _checkedBlocks = new HashSet<uint>();

        public BlockchainHandler(bool isValidator)
        {
            _isValidator = isValidator;

            if (isValidator)
            {
                _miner = new Miner();
                
            }

            _txPool = new TransactionPool();

            _fm = FileManagement.Instance;

            
        }

        public void CheckBlockchain()
        {
            _currentBlockHeight = 0;
            while (true)
            {
                byte[] dta = FileManagement.ReadBlock(_currentBlockHeight);

                if (dta is null)
                {
                    break;
                }
                else
                {
                    if (!_checkedBlocks.Contains(_currentBlockHeight))
                    {
                        Block blk = Parser.ParseBlock(dta);
                        OutputBloomFilter obf = new OutputBloomFilter();

                        foreach (Transaction tx in blk.Transactions)
                        {
                            _fm.AddOutpointsToDB(tx, blk.BlockHeight);
                            _fm.ClearInputsFromDB(tx);

                            foreach (Output ox in tx.Outputs)
                            {
                                obf.AddItem(ox.DestinationPublicKey);
                            }
                        }

                        _fm.WriteOutputBF(obf.GetBytes(), blk.BlockHeight);

                        _checkedBlocks.Add(_currentBlockHeight);
                    }
                    
                    _currentBlockHeight++;
                    _fm.maxBlockNum = _currentBlockHeight;
                }
            }
        }


        public void LoadBlock(byte[] data, byte hopCount)
        {
            Block blk = Parser.ParseBlock(data);

            if (!blk.IsBlockAcceptable())
            {
                return;
            }

            byte[] onDisk = FileManagement.ReadBlock(blk.BlockHeight);

            if (onDisk is null)
            {
                _fm.WriteBlock(blk);
            } else
            {
                Console.WriteLine("Competing block");

            }

            if (_peerManager != null)
            {
                if (hopCount > 0)
                {
                    _ = _peerManager.DiffuseBlock(blk, (byte)(hopCount - 1));
                }

            }
        }

        public void LoadTransaction(byte[] data, byte hopCount)
        {
            Transaction tx = Parser.ParseTransaction(data);

            foreach (Input ix in tx.Inputs)
            {

                (ulong, byte[])? oxData = FileManagement.Instance.RetrieveOutpointData(ix.Outpoint);

                if (!(oxData is null))
                {
                    if (!ix.VerifySignature(oxData.Value.Item2))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Malformed tx.");
                    return;
                }
            }

            _txPool.AddTx(tx);

            if (_peerManager != null)
            {
                if (hopCount > 0)
                {
                    _ = _peerManager.DiffuseTransaction(tx, (byte)(hopCount-1));
                }
                
            }
            
        }

        public void UpdateNodeStatus(bool updateV)
        {
            _isValidator = updateV;

            if (_isValidator)
            {
                _miner = new Miner();
            } else
            {
                _miner = null;
                _isMining = false;
            }

        }

        public void SetMiningAsync(bool isMining)
        {
            
            if (isMining)
            {
                _isMining = isMining;
                List<Transaction> tx = _txPool.GetCandidateTransactions();
                _miner.StartMining(tx, MinerPubKey);

            } else
            {
                if (_isMining)
                {
                    _miner.StopMining();
                }
                _isMining = isMining;
            }

        }

    }
}
