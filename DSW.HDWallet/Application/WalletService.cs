﻿using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMnemonicRepository _mnemonicRepository;

        public WalletService(IWalletRepository walletRepository, IMnemonicRepository mnemonicRepository)
        {
            _walletRepository = walletRepository;
            _mnemonicRepository = mnemonicRepository;
        }
        public Wallet CreateWallet()
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();

            return _walletRepository.Create(mnemo);
        }

        public Wallet CreateWalletWithPassword(string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();

            return _walletRepository.CreateWithPassword(mnemo, password);
        }

        public string RecoverWallet(string secretWords, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(secretWords);

            return _walletRepository.Recover(mnemo, password).ToString();
        }
    }
}
