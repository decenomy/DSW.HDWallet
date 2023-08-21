﻿using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class MnemonicRepository: IMnemonicRepository
    {
        public Mnemonic GenerateMnemonic()
        {
            // TO DO Adicionar suporte a senha

            return new Mnemonic(Wordlist.English, WordCount.Twelve);
        }

        public Mnemonic GetMnemonic(string secretWords)
        {
            return new Mnemonic(secretWords, Wordlist.English);
        }
    }
}
