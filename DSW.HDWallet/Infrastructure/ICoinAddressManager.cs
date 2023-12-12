using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface ICoinAddressManager
    {
        Task<int> GetCoinIndex(string ticker);
        Task<AddressInfo?> GetLastUnusedAddress(string ticker);
        Task<bool> AddressExists(string addressString);
    }
}
