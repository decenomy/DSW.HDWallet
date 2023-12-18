using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IDataStore
    {
        void SaveChanges();
        void AddWallet(Seed seed);
        public void DeleteAllData();
        bool HasWallets();
    }
}
