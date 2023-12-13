using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class WalletManagerService : IWalletManagerService
    {
        public string CreateWallet()
        {
            // Wallet creation logic
            return "New Wallet Created";
        }

        public string RecoverWallet(string mnemonic)
        {
            // Wallet recovery logic
            return "Wallet Recovered";
        }
    }
}
