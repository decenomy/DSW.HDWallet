using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;
using System.Threading.Tasks;

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

        public async Task<string> CreateWallet(int wordCount, string? password = null)
        {
            var mnemonicWordCount = wordCount == 24 ? WordCount.TwentyFour : WordCount.Twelve;
            var createdSeed = walletService.CreateWallet(mnemonicWordCount, password);
            var seed = new Seed { Mnemonic = createdSeed.Mnemonic };

            try
            {
                await storage.AddWallet(seed);
                return seed.Mnemonic ?? "No mnemonic";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> RecoverWallet(string mnemonic, string? password = null)
        {
            var recoveredWallet = walletService.RecoverWallet(mnemonic, password);
            var seed = new Seed { Mnemonic = mnemonic };

            try
            {
                await storage.AddWallet(seed);
                return seed.Mnemonic ?? "Error recovering wallet";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteWallet()
        {
            try
            {
                await storage.DeleteAllData();
                return "Wallet data deleted.";
            }
            catch
            {
                return "Error deleting data from storage.";
            }
        }

        public async Task<bool> HasSeed()
        {
            return await secureStorage.HasSeed();
        }
    }
}
