namespace DSW.HDWallet.ConsoleApp.Domain
{
    public interface IWalletManagerService
    {
        string CreateWallet();
        string RecoverWallet(string mnemonic);
    }
}
