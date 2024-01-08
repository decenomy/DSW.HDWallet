using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class AddressManager : IAddressManager
    {
        private readonly IStorage storage;
        private readonly ICoinRepository coinRepository;

        public AddressManager(IStorage storage, ICoinRepository coinRepository)
        {
            this.storage = storage;
            this.coinRepository = coinRepository;
        }

        public Task<bool> AddressExists(string addressString)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCoinIndex(string ticker)
        {
            throw new NotImplementedException();
        }

        public async Task<AddressInfo?> GetUnusedAddress(string ticker)
        {
            CoinAddress? coinAddress = storage.GetUnusedAddress(ticker);
            if (coinAddress == null)
            {
                // Await the asynchronous call to GetWallet
                Domain.Models.Wallet wallet = await storage.GetWallet(ticker) ?? throw new InvalidOperationException($"Wallet with ticker {ticker} not found.");

                var addressInfo = GetAddress(wallet.PublicKey!, ticker, wallet.CoinIndex + 1, false);

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

            return new AddressInfo()
            {
                Address = coinAddress.Address ?? "",
                Index = coinAddress.AddressIndex,
                IsUsed = coinAddress.IsUsed
            };
        }

        public AddressInfo GetAddress(string pubKey, string ticker, int index, bool isChange = false)
        {
            var changeType = isChange ? 1 : 0;

            Network network = coinRepository.GetNetwork(ticker);
            ExtPubKey extPubKey = ExtPubKey.Parse(pubKey, network);

            var keypath = $"{changeType}/{index}";

            var address = extPubKey.Derive(new KeyPath(keypath))
                                    .GetPublicKey()
                                    .GetAddress(ScriptPubKeyType.Legacy, network);

            AddressInfo deriveKeyDetails = new()
            {
                Address = address.ToString(),
                Index = index
            };

            return deriveKeyDetails;
        }
    }
}
