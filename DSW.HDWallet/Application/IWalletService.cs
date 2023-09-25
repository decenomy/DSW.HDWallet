using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using Words = NBitcoin.WordCount;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet(Words wordCount);
        Wallet CreateWalletWithPassword(Words wordCount, string? password = null);
        string RecoverWallet(string secretWords, string? password = null);
        DeriveKeyDetails CreateDerivedKey(CoinType coinType, string masterKey, int index, string? password = null);
        PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex);
        DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index);
        Task<AddressObject> GetAddressAsync(string coin, string address);
        Task<TransactionObject> GetTransactionAsync(string coin, string txid);
        Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid);
        Task GetWSTransactionAsync(string coin, string txId);
    }
}
