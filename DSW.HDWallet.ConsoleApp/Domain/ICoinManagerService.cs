using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface ICoinManagerService
    {
        IEnumerable<ICoinExtension> GetAvailableCoins();
        bool AddCoin(string ticker, string? password = null);
        List<Wallet> GetWalletCoins();
        void SendCoins(string ticker, decimal numberOfCoins, string address, string? password);

    }
}
