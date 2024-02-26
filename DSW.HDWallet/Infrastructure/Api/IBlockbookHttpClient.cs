using DSW.HDWallet.Domain.ApiObjects;

namespace DSW.HDWallet.Infrastructure.Api
{
    public interface IBlockbookHttpClient
    {
        Task<AddressObject> GetAddressAsync(string coin, string address);
        Task<TransactionObject> GetTransactionAsync(string coin, string txid);
        Task<FeeResultObject> GetFeeEstimation(string coin, int blockMumber);
        Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid);
        Task<BlockHashObject> GetBlockHash(string coin, string blockHeight);
        Task<XpubObject> GetXpub(string coin, string xpub, int page = 1, int pageSize = 1000);
        Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed = false);
        Task<TransactionSendResponse> SendTransaction(string ticker, string rawTransaction);
    }
}