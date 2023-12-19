namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IWalletManagerService
    {
        string CreateWallet(int wordCount, string? password = null);
        string RecoverWallet(string mnemonic, string? password = null);
        string DeleteWallet();
        bool HasSeed();
    }
}
