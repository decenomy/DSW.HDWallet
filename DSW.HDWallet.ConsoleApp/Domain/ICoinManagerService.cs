using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface ICoinManagerService
    {
        IEnumerable<ICoinExtension> GetAvailableCoins();
        bool AddCoin(string ticker, string? password = null);
        List<Wallet> GetWalletCoins();
    }
}
