﻿using DSW.HDWallet.Application.Objects;
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

        public Task<IEnumerable<object>> GetTransactions(string v)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> SendCoins(string ticker, decimal numberOfCoins, string address, string? password)
        {
            var recoveredWallet = walletService.RecoverWallet(secureStorage.GetMnemonic().Result, password);

            TransactionDetails transactionDetails = await walletService.GenerateTransaction(ticker, recoveredWallet, SatoshiConverter.ToSatoshi(Convert.ToInt64(numberOfCoins)), address);

            if (transactionDetails.Transaction == null)
            {
                return OperationResult.Fail("Transaction object is null.");
            }
            else
            {
                string rawTransaction = transactionDetails.Transaction.ToHex();

                var response = await blockbookHttpClient.SendTransaction(ticker, rawTransaction);

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
