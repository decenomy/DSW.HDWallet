using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class CoinAddressManager : ICoinAddressManager
    {
        public Task<bool> AddressExists(string addressString)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCoinIndex(string ticker)
        {
            throw new NotImplementedException();
        }

        public Task<AddressInfo?> GetLastUnusedAddress(string ticker)
        {
            throw new NotImplementedException();
        }
    }
}
