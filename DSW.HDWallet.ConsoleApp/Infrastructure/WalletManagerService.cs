using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class WalletManagerService : IWalletManagerService
    {
        private readonly IDataStore dataStore;
        private readonly IWalletService walletService;

        public WalletManagerService(IDataStore dataStore, 
            IWalletService walletService)
        {
            this.dataStore = dataStore;
            this.walletService = walletService;
        }
        public string CreateWallet()
        {
            var createdWallet = walletService.CreateWallet(NBitcoin.WordCount.Twelve, null);
            var wallet = new Wallet { Mnemonic = createdWallet.Mnemonic };
            try
            {
                dataStore.Wallets.Add(wallet);
                dataStore.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return wallet.Mnemonic ?? "No mnemonic";
        }

        public string RecoverWallet(string mnemonic)
        {
            var recoveredWallet = walletService.RecoverWallet(mnemonic);
            var wallet = new Wallet { Mnemonic = recoveredWallet };
            try
            {
                dataStore.Wallets.Add(wallet);
                dataStore.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return wallet.Mnemonic ?? "Error recovering wallet";
        }
    }
}
