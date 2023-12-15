using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Domain.Models;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class WalletManagerService : IWalletManagerService
    {
        private readonly IDataStore dataStore;
        private readonly IWalletService walletService;

        public WalletManagerService(IDataStore dataStore, IWalletService walletService)
        {
            this.dataStore = dataStore;
            this.walletService = walletService;
        }

        public string CreateWallet(string? password = null)
        {
            var createdWallet = walletService.CreateWallet(NBitcoin.WordCount.Twelve, password);
            var wallet = new Wallet { Mnemonic = createdWallet.Mnemonic };
            try
            {
                dataStore.AddWallet(wallet);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return wallet.Mnemonic ?? "No mnemonic";
        }

        public string RecoverWallet(string mnemonic, string? password = null)
        {
            var recoveredWallet = walletService.RecoverWallet(mnemonic, password);
            var wallet = new Wallet { Mnemonic = recoveredWallet };
            try
            {
                dataStore.AddWallet(wallet);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return wallet.Mnemonic ?? "Error recovering wallet";
        }

        public string DeleteWallet()
        {
            try
            {
                dataStore.DeleteAllData();
                return "Wallet data deleted.";
            }
            catch
            {
                return "Error deleting data from storage.";
            }
        }
    }

}
