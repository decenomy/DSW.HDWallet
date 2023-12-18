using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class WalletManagerService : IWalletManagerService
    {
        private readonly IStorage storage;
        private readonly IWalletService walletService;

        public WalletManagerService(IStorage storage, IWalletService walletService)
        {
            this.storage = storage;
            this.walletService = walletService;
        }

        public string CreateWallet(string? password = null)
        {
            var createdSeed = walletService.CreateWallet(NBitcoin.WordCount.Twelve, password);
            var seed = new Seed { Mnemonic = createdSeed.Mnemonic };
            try
            {
                storage.AddWallet(seed);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return seed.Mnemonic ?? "No mnemonic";
        }

        public string RecoverWallet(string mnemonic, string? password = null)
        {
            var recoveredWallet = walletService.RecoverWallet(mnemonic, password);
            var seed = new Seed { Mnemonic = recoveredWallet };
            try
            {
                storage.AddWallet(seed);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return seed.Mnemonic ?? "Error recovering wallet";
        }

        public string DeleteWallet()
        {
            try
            {
                storage.DeleteAllData();
                return "Wallet data deleted.";
            }
            catch
            {
                return "Error deleting data from storage.";
            }
        }
    }

}
