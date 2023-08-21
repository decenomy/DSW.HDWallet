using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Application
{
    public interface IWalletService
    {
        Wallet CreateWallet();
        string RecoverWallet(string secretWords);
    }
}
