using DSW.HDWallet.Application.Objects;
using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;

namespace DSW.HDWallet.Application
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

        public async Task<OperationResult> SendCoins(string ticker, decimal numberOfCoins, string address, string? password)
        {
            var recoveredWallet = walletService.RecoverWallet(secureStorage.GetMnemonic().Result, password);

            TransactionDetails transactionDetails = await walletService.GenerateTransaction(ticker, recoveredWallet, SatoshiConverter.ToSatoshi(Convert.ToInt64(numberOfCoins)), address);

            if (transactionDetails.Transaction == null)
            {
                return OperationResult.Fail(transactionDetails.Message ?? "Transaction object is null.");
            }
            else
            {
                string rawTransaction = transactionDetails.Transaction.ToHex();

                var response = await blockbookHttpClient.SendTransaction(ticker, rawTransaction);

                if (response.Error == null)
                {

                    foreach (var utxo in transactionDetails.Utxos ?? Enumerable.Empty<UtxoObject>())
                    {
                        // Add addresses that are not in our database yet
                        var addressResult = await storage.GetAddressByAddress(utxo.Address ?? "");
                        if (addressResult == null)
                        {
                            var wallet = await storage.GetWallet(ticker);

                            CoinAddress newAddress = new CoinAddress
                            {
                                Ticker = ticker,
                                Address = utxo.Address,
                                IsUsed = true,
                                AddressIndex = wallet!.CoinIndex + 1,
                                IsChange = false
                            };

                            await storage.AddCoinAddress(newAddress);
                            await storage.IncrementCoinIndex(ticker);
                        }
                    }

                    CoinAddress changeAddress = new()
                    {
                        Ticker = ticker,
                        Address = transactionDetails.ChangeAddress?.Address,
                        AddressIndex = transactionDetails.ChangeAddress?.Index ?? 0,
                        IsUsed = true,
                        IsChange = true
                    };

                    var existingChangeAddress = await storage.GetAddressByAddress(changeAddress.Address ?? "");

                    if (existingChangeAddress == null)
                    {
                        await storage.AddCoinAddress(changeAddress);
                        await storage.IncrementCoinIndex(ticker);
                    }
                    else
                    {
                        await storage.UpdateAddressUsed(changeAddress);
                    }

                    return OperationResult.Ok("Transaction submitted successfully.");
                }
                else
                {
                    return OperationResult.Fail($"Error: {transactionDetails.Message}");
                }
            }

        }
    }
}
