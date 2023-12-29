using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface IWalletManager
    {
        string CreateWallet(int wordCount, string? password = null);
        string RecoverWallet(string mnemonic, string? password = null);
        string DeleteWallet();
        bool HasSeed();
    }
}
