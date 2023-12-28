using DSW.HDWallet.Application;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;

namespace DSW.HDWallet.Infrastructure
{
    public class TransactionManager : ITransactionManager
    {
        private readonly IStorage storage;
        private readonly ISecureStorage secureStorage;
        private readonly IWalletService walletService;
        private readonly IBlockbookHttpClient blockbookHttpClient;

        public TransactionManager(IStorage storage, 
            ISecureStorage secureStorage, 
            IWalletService walletService,
            IBlockbookHttpClient blockbookHttpClient)
        {
            this.storage = storage;
            this.secureStorage = secureStorage;
            this.walletService = walletService;
            this.blockbookHttpClient = blockbookHttpClient;
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
                string rawTransaction = transactionDetails.Transaction.ToHex();

                var response = blockbookHttpClient.SendTransaction(rawTransaction).Result;

                if (response.Error == null)
                {
                    CoinAddress changeAddress = new()
                    {
                        Ticker = ticker,
                        Address = transactionDetails.ChangeAddress?.Address,
                        AddressIndex = transactionDetails.ChangeAddress?.Index ?? 0,
                        IsUsed = true,
                        IsChange = true
                    };

                    if (storage.GetAddressByAddress(changeAddress.Address ?? "") == null)
                    {
                        storage.AddCoinAddress(changeAddress);
                        storage.IncrementCoinIndex(ticker);
                    }
                    else
                    {
                        storage.UpdateAddressUsed(changeAddress);
                    }

                    Console.WriteLine("Transaction submitted successfully, but no result was returned.");
                }
                else
                {
                    Console.WriteLine("response.Error.Message");
                }
            }

        }
    }
}
