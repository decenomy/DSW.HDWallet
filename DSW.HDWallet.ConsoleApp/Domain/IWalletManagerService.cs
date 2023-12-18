namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IWalletManagerService
    {
        string CreateWallet(string? password = null);
        string RecoverWallet(string mnemonic, string? password = null);
        string DeleteWallet();
        bool HasSeed();
    }
}
