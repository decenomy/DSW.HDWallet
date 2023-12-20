using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using Wallet = DSW.HDWallet.Domain.Models.Wallet;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    public class CoinManagerService : ICoinManagerService
    {
        private readonly ICoinRepository coinRepository;
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;
        private readonly IBlockbookHttpClient blockbookHttpClient;

        public CoinManagerService(
            ICoinRepository coinRepository,
            IStorage storage,
            ISecureStorage secureStorage,
            IWalletService walletService,
            IBlockbookHttpClient blockbookHttpClient)
        {
            this.coinRepository = coinRepository;
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
            this.blockbookHttpClient = blockbookHttpClient;
        }

        public IEnumerable<ICoinExtension> GetAvailableCoins()
        {
            var allWallets = storage.GetAllWallets();

            // Return coins that are not already in the wallets
            return coinRepository.Coins.Where(coin => !allWallets.Any(wallet => wallet.Ticker == coin.Ticker));
        }

        public bool AddCoin(string ticker, string? password = null)
        {
            var mnemonic = secureStorage.GetMnemonic();

            var seedHex = walletService.RecoverWallet(mnemonic, password);

            PubKeyDetails pubKeyDetails = walletService.GeneratePubkey(ticker, seedHex ?? "");

            Wallet wallet = new()
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
                AddressInfo addressInfo = walletService.GetAddress(pubKeyDetails.PubKey, ticker, pubKeyDetails.Index, false);

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

        public List<Wallet> GetWalletCoins()
        {
            return storage.GetAllWallets();
        }

        public void SendCoins(string ticker, decimal numberOfCoins, string address, string? password)
        {
            secureStorage.GetMnemonic();
            var recoveredWallet = walletService.RecoverWallet(secureStorage.GetMnemonic(), password);

            TransactionDetails transactionDetails = walletService.GenerateTransaction(ticker, recoveredWallet, Convert.ToInt64(numberOfCoins), address).Result;


            if (transactionDetails.Transaction == null)
            {
                Console.WriteLine("Transaction object is null.");
            }
            else
            {
                //string rawTransaction = transactionDetails.Transaction.ToHex();

                //var response = blockbookHttpClient.SendTransaction(rawTransaction).Result;

                //if (response.Error == null)
                //{
                //    CoinAddressDataObject changeAddress = new()
                //    {
                //        Ticker = ticker,
                //        Address = transactionDetails.ChangeAddress?.Address,
                //        AddressIndex = transactionDetails.ChangeAddress?.Index ?? 0,
                //        IsUsed = true,
                //        IsChange = true
                //    };

                //    if (await repository.GetAddressByAddress(changeAddress.Address ?? "") == null)
                //    {
                //        await repository.AddCoinAddress(changeAddress);
                //        await repository.IncrementCoinIndex(ticker);
                //    }
                //    else
                //    {
                //        await repository.UpdateAddressUsed(changeAddress);
                //    }

                //    return (true, response.Result ?? "Transaction submitted successfully, but no result was returned.");
                //}
                //else
                //{
                //    logger.LogError("Error sending transaction: {ErrorMessage}", response.Error.Message);
                //    return (false, response.Error.Message ?? "Unknown error occurred.");
                //}
            }

        }
    }

}
