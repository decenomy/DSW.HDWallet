using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Domain.WSObject;
using NBitcoin;
using Words = NBitcoin.WordCount;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet(Words wordCount, string? password = null);
        string GetSeedHex(Mnemonic mnemo, string? password = null);
        string RecoverWallet(string secretWords, string? password = null);
        PubKeyDetails GeneratePubkey(string ticker, string seedHex);
        Task<AddressObject> GetAddressAsync(string coin, string address);
        Task<TransactionObject> GetTransactionAsync(string coin, string txid);
        Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid);
        Task<BlockHashObject> GetBlockHash(string coin, string blockHeight);
        Task<XpubObject> GetXpub(string coin, string xpub, int page = 1, int pageSize = 1000);
        Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed = false);

        //Task<WSTransactionObject> GetWSTransactionAsync(string coin, string txId);
        //Task<WSSubscribeObject> SubscribeNewTransaction(string coin);

        Task<TransactionDetails> GenerateTransaction(string ticker, string seedHex, long amountToSend, string toAddress, long fee = 0);

        bool ValidateAddress(string ticker, string address);
    }
}