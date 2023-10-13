using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Domain.WSObject;
using Words = NBitcoin.WordCount;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet(Words wordCount);
        Wallet CreateWalletWithPassword(Words wordCount, string? password = null);
        string RecoverWallet(string secretWords, string? password = null);
        DeriveKeyDetails CreateDerivedKey(CoinType coinType, string masterKey, int index, string? password = null, bool isNetworkTest = false);
        PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex, bool isNetworkTest = false);
        DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index, bool isNetworkTest = false);
        Task<AddressObject> GetAddressAsync(string coin, string address);
        Task<TransactionObject> GetTransactionAsync(string coin, string txid);
        Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid);
        Task<BlockHashObject> GetBlockHash(string coin, string blockHeight);
        Task<XpubObject> GetXpub(string coin, string xpub);
        Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed = false);

        Task<WSTransactionObject> GetWSTransactionAsync(string coin, string txId);
        Task<WSSubscribeObject> SubscribeNewTransaction(string coin);

        Task<TransactionDetails> GenerateTransactionAsync(CoinType coinType, long amountToSend, string seedHex, string fromAddress, string toAddress);
    }
}