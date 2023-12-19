using DSW.HDWallet.Domain.Coins;

namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface ICoinManagerService
    {
        IEnumerable<ICoinExtension> GetAvailableCoins();
        bool AddCoin(string ticker, string? password = null);
    }
}
