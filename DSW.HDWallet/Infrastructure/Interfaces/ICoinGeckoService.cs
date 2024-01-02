using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinGeckoService
    {
        Task<string> GetRatesAsync(IEnumerable<string> tickers, IEnumerable<string> currencies);
    }
}
