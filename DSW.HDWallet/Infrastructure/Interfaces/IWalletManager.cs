using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface IWalletManager
    {
        Task<string> CreateWallet(int wordCount, string? password = null);
        Task<string> RecoverWallet(string mnemonic, string? password = null);
        Task<string> DeleteWallet();
        Task<bool> HasSeed();
    }
}
