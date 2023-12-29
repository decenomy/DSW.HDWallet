using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinManager
    {
        IEnumerable<ICoinExtension> GetAvailableCoins();
        bool AddCoin(string ticker, string? password = null);
    }
}
