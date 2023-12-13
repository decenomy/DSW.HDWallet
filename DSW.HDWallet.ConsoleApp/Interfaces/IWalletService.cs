namespace DSW.HDWallet.ConsoleApp.Interfaces
{
    public interface IWalletService
    {
        string CreateWallet();
        string RecoverWallet(string mnemonic);
    }
}
