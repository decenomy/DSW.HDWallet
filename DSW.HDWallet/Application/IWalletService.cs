using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet();
        Wallet CreateWalletWithPassword(string? password = null);
        string RecoverWallet(string secretWords, string? password = null);
        DeriveKeyDetails CreateDerivedKey(CoinType coinType, string masterKey, int index);
    }
}
