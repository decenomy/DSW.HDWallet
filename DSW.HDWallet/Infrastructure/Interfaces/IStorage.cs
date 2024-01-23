using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface IStorage
    {
        Task AddWallet(Seed seed);
        Task DeleteAllData();
        Task<bool> AddCoin(Wallet wallet);
        Task<bool> AddAddress(CoinAddress coinAddress);
        Task<IEnumerable<Wallet>> GetAllWallets();
        Task<IEnumerable<Rate>> GetAllRates();
        Task AddCoinAddress(CoinAddress coinAddress);
        Task IncrementCoinIndex(string ticker);
        Task<CoinAddress> GetAddressByAddress(string address);
        Task UpdateAddressUsed(CoinAddress coinAddress);
        Task<CoinAddress?> GetUnusedAddress(string ticker);
        Task<Wallet?> GetWallet(string ticker);
        Task SaveRates(Rate rate);
        Task SaveBalance(Wallet coin);
        Task AddTransaction(TransactionRecord transaction);
        Task<TransactionRecord?> GetTransactionByTxId(string txid);
        Task<IEnumerable<CoinAddress>> GetAddressesByTicker(string ticker);
        Task<IEnumerable<TransactionRecord>> GetTransactions(string? ticker = null, int pageNumber = 1, int pageSize = 10, TransactionType? transactionType = null);
    }
}
