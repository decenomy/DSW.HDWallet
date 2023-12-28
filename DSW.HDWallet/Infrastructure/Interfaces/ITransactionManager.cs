using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ITransactionManager
    {
        void SendCoins(string ticker, decimal numberOfCoins, string address, string? password);
    }
}
