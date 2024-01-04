using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface IRatesUpdateService
    {
        Task UpdateRatesAsync();
    }
}
