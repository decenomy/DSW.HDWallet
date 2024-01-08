using DSW.HDWallet.Application.Features;
using DSW.HDWallet.Application.Objects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class CoinManager : ICoinManager, ICoinBalanceRetriever
    {
        private readonly ICoinRepository coinRepository;
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;
        private readonly IAddressManager addressManager;

        public CoinManager(
            ICoinRepository coinRepository,
            IStorage storage,
            ISecureStorage secureStorage,
            IWalletService walletService,
            IAddressManager addressManager)
        {
            this.coinRepository = coinRepository;
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
            this.addressManager = addressManager;
        }

        public async Task<IEnumerable<ICoinExtension>> GetAvailableCoins()
        {
            var allWallets = await storage.GetAllWallets();

            // Return coins that are not already in the wallets
            return coinRepository.Coins.Where(coin => !allWallets.Any(wallet => wallet.Ticker == coin.Ticker));
        }

        public bool AddCoin(string ticker, string? password = null)
        {
            var mnemonic = secureStorage.GetMnemonic();

            var seedHex = walletService.RecoverWallet(mnemonic, password);

            PubKeyDetails pubKeyDetails = walletService.GeneratePubkey(ticker, seedHex ?? "");

            Domain.Models.Wallet wallet = new()
            {
                Ticker = ticker,
                PublicKey = pubKeyDetails.PubKey,
                Path = pubKeyDetails.Path,
                CoinIndex = pubKeyDetails.Index,
                Balance = 0
            };

            bool coinAddSuccess = storage.AddCoin(wallet);

            if (coinAddSuccess)
            {
                AddressInfo addressInfo = addressManager.GetAddress(pubKeyDetails.PubKey, ticker, pubKeyDetails.Index, false);

                CoinAddress walletAddress = new()
                {
                    Ticker = ticker,
                    Address = addressInfo.Address,
                    AddressIndex = addressInfo.Index,
                    IsChange = false
                };

                if (storage.AddAddress(walletAddress))
                    return true;
            }

            return false;
        }

        public Dictionary<string, string> GetCoinGeckoIds()
        {
            var tickers = new Dictionary<string, string>();
            var allCoins = coinRepository.Coins;

            foreach (var coin in allCoins)
                tickers.Add(coin.Ticker!, coin.CoinGeckoId!);

            return tickers;
        }

        public async Task<CoinBalance> GetCoinBalance(string ticker)
        {
            var wallet = await storage.GetWallet(ticker);
            if (wallet == null)
            {
                throw new InvalidOperationException($"Wallet with ticker {ticker} not found.");
            }

            decimal balance = SatoshiConverter.FromSubSatoshi(wallet.Balance ?? 0);
            decimal unconfirmedBalance = SatoshiConverter.FromSubSatoshi(wallet.UnconfirmedBalance ?? 0);

            decimal lockedBalance = 0m;

            return new CoinBalance(balance, unconfirmedBalance, lockedBalance);
        }

        public Task<decimal> GetCurrencyBalance(string currency, string? ticker = null)
        {
            throw new NotImplementedException();
        }

    }
}
