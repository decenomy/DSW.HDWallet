using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class CoinAddressManager : ICoinAddressManager
    {
        private readonly IStorage storage;
        private readonly IWalletService walletService;

        public CoinAddressManager(IStorage storage, IWalletService walletService)
        {
            this.storage = storage;
            this.walletService = walletService;
        }

        public Task<bool> AddressExists(string addressString)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCoinIndex(string ticker)
        {
            throw new NotImplementedException();
        }

        public Task<AddressInfo?> GetUnusedAddress(string ticker)
        {
            CoinAddress? coinAddress = storage.GetUnusedAddress(ticker);
            if (coinAddress == null)
            {
                HDWallet.Domain.Models.Wallet wallet = storage.GetWallet(ticker)!;

                var addressInfo = walletService.GetAddress(wallet.PublicKey!, ticker, wallet.CoinIndex + 1, false);

                coinAddress = new CoinAddress()
                {
                    Address = addressInfo.Address,
                    AddressIndex = wallet.CoinIndex,
                    Ticker = ticker,
                    IsChange = false,
                    IsUsed = false
                };

                storage.AddAddress(coinAddress);
                storage.IncrementCoinIndex(ticker);
                
            }

            return Task.FromResult<AddressInfo?>(
                new AddressInfo() { 
                    Address = coinAddress.Address ?? "",
                    Index = coinAddress.AddressIndex,
                    IsUsed = coinAddress.IsUsed
                });
        }
    }
}
