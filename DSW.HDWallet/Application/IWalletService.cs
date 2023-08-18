using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet();
        BitcoinAddress RecoverWallet(string secretWords);
    }
}
