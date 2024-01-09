using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinManager
    {
        Task<IEnumerable<ICoinExtension>> GetAvailableCoins();
        Task<bool> AddCoin(string ticker, string? password = null);
        Task<Dictionary<string, string>> GetCoinGeckoIds();
    }
}
