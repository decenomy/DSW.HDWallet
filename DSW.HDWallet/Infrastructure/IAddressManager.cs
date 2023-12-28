using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure
{
    public interface IAddressManager
    {
        Task<int> GetCoinIndex(string ticker);
        Task<bool> AddressExists(string addressString);
        Task<AddressInfo?> GetUnusedAddress(string ticker);
        AddressInfo GetAddress(string pubKey, string ticker, int Index, bool IsChange = false);
    }
}
