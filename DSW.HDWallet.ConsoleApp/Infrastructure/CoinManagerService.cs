﻿using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using Wallet = DSW.HDWallet.Domain.Models.Wallet;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class CoinManagerService : ICoinManagerService
    {
        private readonly ICoinRepository coinRepository;
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;

        public CoinManagerService(
            ICoinRepository coinRepository, 
            IStorage storage, 
            ISecureStorage secureStorage, 
            IWalletService walletService)
        {
            this.coinRepository = coinRepository;
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
        }

        public IEnumerable<ICoinExtension> GetAvailableCoins()
        {
            var allWallets = storage.GetAllWallets();

            // Return coins that are not already in the wallets
            return coinRepository.Coins.Where(coin => !allWallets.Any(wallet => wallet.Ticker == coin.Ticker));
        }

        public bool AddCoin(string ticker, string? password = null)
        {
            var mnemonic = secureStorage.GetMnemonic();

            var seedHex = walletService.RecoverWallet(mnemonic, password);

            PubKeyDetails pubKeyDetails = walletService.GeneratePubkey(ticker, seedHex ?? "");

            Wallet wallet = new()
            {
                Ticker = ticker,
                PublicKey = pubKeyDetails.PubKey,
                Path = pubKeyDetails.Path,
                CoinIndex = pubKeyDetails.Index,
                Balance = 0
            };

            bool coinAddSuccess = storage.AddCoin(wallet);

            if (coinAddSuccess)
            {
                AddressInfo addressInfo = walletService.GetAddress(pubKeyDetails.PubKey, ticker, pubKeyDetails.Index, false);

                CoinAddress walletAddress = new()
                {
                    Ticker = ticker,
                    Address = addressInfo.Address,
                    AddressIndex = addressInfo.Index,
                    IsChange = false
                };

                if (storage.AddAddress(walletAddress))
                    return true;
            }

            return false;
        }
    }

}
