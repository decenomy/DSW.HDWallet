﻿using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure
{
    public interface IStorage
    {
        void AddWallet(Seed seed);
        public void DeleteAllData();
        bool AddCoin(Wallet wallet);
        bool AddAddress(CoinAddress coinAddress);
        List<Wallet> GetAllWallets();
        void AddCoinAddress(CoinAddress coinAddress);
        void IncrementCoinIndex(string ticker);
        CoinAddress GetAddressByAddress(string address);
        void UpdateAddressUsed(CoinAddress coinAddress);
    }
}
