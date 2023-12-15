using DSW.HDWallet.ConsoleApp.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IDataStore
    {
        void SaveChanges();
        void AddWallet(Wallet wallet);
        public void DeleteAllData();
        bool HasWallets();
    }
}
