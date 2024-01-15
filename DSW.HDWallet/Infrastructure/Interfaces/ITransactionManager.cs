using DSW.HDWallet.Application.Objects;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ITransactionManager
    {
        Task<IEnumerable<object>> GetTransactions(string v);
        Task<OperationResult> SendCoins(string ticker, decimal numberOfCoins, string address, string? password);
    }
}
