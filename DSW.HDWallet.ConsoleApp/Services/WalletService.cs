using DSW.HDWallet.ConsoleApp.Interfaces;

namespace DSW.HDWallet.ConsoleApp.Services
{
    public class WalletService : IWalletService
    {
        public string CreateWallet()
        {
            return "Wallet Created";
        }

        public string RecoverWallet(string mnemonic)
        {
            return "Wallet Recovered";
        }
    }
}
