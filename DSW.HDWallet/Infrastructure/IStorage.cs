using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure
{
    public interface IStorage
    {
        void AddWallet(Seed seed);
        public void DeleteAllData();
        bool AddCoin(Wallet wallet);
        bool AddAddress(CoinAddress coinAddress);
    }
}
