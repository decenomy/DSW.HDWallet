using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinManager
    {
        Task<IEnumerable<ICoinExtension>> GetAvailableCoins();
        bool AddCoin(string ticker, string? password = null);
        Dictionary<string, string> GetCoinGeckoIds();
        long GetCoinBalance(string ticker);
    }
}
