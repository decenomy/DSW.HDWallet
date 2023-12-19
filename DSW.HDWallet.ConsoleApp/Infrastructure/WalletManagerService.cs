using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class WalletManagerService : IWalletManagerService
    {
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;

        public WalletManagerService(IStorage storage, ISecureStorage secureStorage, IWalletService walletService)
        {
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
        }

        public string CreateWallet(int wordCount, string? password = null)
        {
            var mnemonicWordCount = wordCount == 24 ? NBitcoin.WordCount.TwentyFour : NBitcoin.WordCount.Twelve;
            var createdSeed = walletService.CreateWallet(mnemonicWordCount, password);
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

        public bool HasSeed()
        {
            return secureStorage.HasSeed();
        }
    }

}
