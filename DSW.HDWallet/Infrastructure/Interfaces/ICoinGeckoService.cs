using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinGeckoService
    {
        Task<string> GetRatesAsync(IEnumerable<string> coinGeckoIds, IEnumerable<string> currencies);
    }
}
