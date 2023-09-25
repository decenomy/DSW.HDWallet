using DSW.HDWallet.Domain.ApiObjects;

namespace DSW.HDWallet.Infrastructure.Api
{
    public interface IApiDecenomyExplorerRepository
    {
        Task<AddressObject> GetAddressAsync(string coin, string address);
        Task<TransactionObject> GetTransactionAsync(string coin, string txid);
        Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid);
    }
}
