using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface IStorage
    {
        void AddWallet(Seed seed);
        public void DeleteAllData();
        bool AddCoin(Wallet wallet);
        bool AddAddress(CoinAddress coinAddress);
        Task<IEnumerable<Wallet>> GetAllWallets();
        void AddCoinAddress(CoinAddress coinAddress);
        void IncrementCoinIndex(string ticker);
        CoinAddress GetAddressByAddress(string address);
        void UpdateAddressUsed(CoinAddress coinAddress);
        CoinAddress? GetUnusedAddress(string ticker);
        Task<Wallet?> GetWallet(string ticker);
        Task SaveRates(Rate rate);
        Task SaveBalance(Wallet coin);
    }
}
