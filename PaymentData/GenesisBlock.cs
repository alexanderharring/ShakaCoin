﻿using ShakaCoin.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShakaCoin.PaymentData
{
    internal class GenesisBlock
    {

        public static Block MakeGenesisBlock()
        {
            Block gBlock = new Block();

            string pubKey = "50C934260B7B86F04A3A9A8B0538C044C85BFAFEF2C028564B267987BF2C0DA0";

            byte version = 0b10000000; // coinbase transaction

            ulong initialBR = (ulong)256;

            Transaction gTransaction = new Transaction(version);

            Output ox = new Output(initialBR, Hasher.GetBytesFromHexStringQuick(pubKey));

            Input ix = new Input(new byte[32], 0xFF);

            byte[] fakeSignature = new byte[64];

            byte[] copySource = Hasher.GetBytesQuick("This is the coinbase transaction of the genesis block. :)");
            Array.Copy(copySource, fakeSignature, copySource.Length);

            ix.AddSignature(fakeSignature);

            gTransaction.AddInput(ix);
            gTransaction.AddOutput(ox);

            gBlock.AddTransaction(gTransaction);
            gBlock.BlockHeight = 0;

            byte[] Target = new byte[32];
            Target[3] = 0xF0;
            gBlock.Target = Target;
            gBlock.Version = 0x00;
            gBlock.TimeStamp = 1741334817;

            WorkingBlock _wb = new WorkingBlock(gBlock);
            _wb.GenerateMerkleRoot();

            gBlock.MerkleRoot = _wb.MerkleRoot;
            gBlock.MiningIncrement = 19953029;

            return gBlock;
        }
    }
}
