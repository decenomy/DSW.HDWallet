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
    }
}
