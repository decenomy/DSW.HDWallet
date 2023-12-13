using DSW.HDWallet.ConsoleApp.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IDataStore
    {
        void SaveChanges();
        List<Wallet> Wallets { get; }
        List<CoinAddress> CoinAddresses { get; }
        List<Rate> Rates { get; }
        List<WalletCoin> WalletCoins { get; }
        List<Setting> Settings { get; }
    }
}
