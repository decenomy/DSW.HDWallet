using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class CoinManagerService : ICoinManagerService
    {
        private readonly ICoinRepository coinRepository;
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;

        public CoinManagerService(ICoinRepository coinRepository, IStorage storage, ISecureStorage secureStorage)
        {
            this.coinRepository = coinRepository;
            this.storage = storage;
            this.secureStorage = secureStorage;
        }

        public IEnumerable<ICoinExtension> GetAvailableCoins()
        {
            return coinRepository.Coins;
        }

        public bool AddCoin(string ticker, string? password = null)
        {
            throw new NotImplementedException();
            //var mnemo = secureStorage.GetMnemonic();

            //return _walletService.RecoverWallet(mnemo ?? "", password);
        }
    }

}
