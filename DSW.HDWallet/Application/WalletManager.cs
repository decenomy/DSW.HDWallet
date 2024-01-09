using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class WalletManager : IWalletManager
    {
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;

        public WalletManager(IStorage storage, ISecureStorage secureStorage, IWalletService walletService)
        {
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
        }

        public string CreateWallet(int wordCount, string? password = null)
        {
            var mnemonicWordCount = wordCount == 24 ? WordCount.TwentyFour : WordCount.Twelve;
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
            var seed = new Seed { Mnemonic = mnemonic };
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
            return secureStorage.HasSeed().Result;
        }
    }

}
