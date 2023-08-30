using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet(CoinType coinType);
        Wallet CreateWalletWithPassword(CoinType coinType, string? password = null);
        string RecoverWallet(CoinType coinType, string secretWords, string? password = null);
        BitcoinExtKey CreateDerivedKey(CoinType coinType, int index);
    }
}
