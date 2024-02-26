using DSW.HDWallet.Application.Objects;
using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ICoinBalanceRetriever
    {
        Task<CoinBalance> GetCoinBalance(string ticker);

        Task<decimal> GetCurrencyBalance(string currency, string? ticker = null);
    }
}
