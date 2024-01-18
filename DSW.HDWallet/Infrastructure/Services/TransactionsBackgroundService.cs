using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class TransactionsBackgroundService : BaseBackgroundService<TransactionsBackgroundService>
    {
        private readonly IBlockbookHttpClient blockbookHttpClient;
        private readonly IStorage storage;

        public TransactionsBackgroundService(
            IBlockbookHttpClient blockbookHttpClient,
            IStorage storage,
            ILogger<TransactionsBackgroundService> logger
        ) : base(logger, "0 */1 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.blockbookHttpClient = blockbookHttpClient;
            this.storage = storage;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Rates Update Service executing.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var wallets = await storage.GetAllWallets();
                    foreach (var wallet in wallets)
                    {
                        var addresses = storage.GetAddressesByTicker(wallet.Ticker);
                        var xpub = await blockbookHttpClient.GetXpub(wallet.Ticker!, wallet.PublicKey!);

                        foreach (var txid in xpub.Txids!)
                        {
                            var existingTransaction = await storage.GetTransactionByTxId(txid);
                            if (existingTransaction == null)
                            {
                                var transactionDetails = await blockbookHttpClient.GetTransactionAsync(wallet.Ticker!, txid);

                                var transactionType = DetermineTransactionType(transactionDetails, wallet.PublicKey);
                                var transactionRecord = new TransactionRecord
                                {
                                    TxId = transactionDetails.Txid,
                                    Ticker = wallet.Ticker,
                                    Type = transactionType, 
                                    Amount = CalculateTransactionAmount(transactionDetails, wallet.PublicKey, transactionType), 
                                    FromAddress = transactionDetails.Vin.FirstOrDefault()?.Addresses.FirstOrDefault(),
                                    ToAddress = transactionDetails.Vout.FirstOrDefault()?.Addresses.FirstOrDefault(),
                                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(transactionDetails.BlockTime).UtcDateTime,
                                    IsConfirmed = transactionDetails.Confirmations > 0,
                                    TransactionFee = Convert.ToDecimal(transactionDetails.Fees) / 100000000, // Example conversion (depends on your decimal implementation)
                                    Notes = "" // Any additional information or leave empty
                                };

                                //await storage.AddTransaction(transactionRecord);
                            }
                        }
                    }

                    var t = DateTime.Now;
                    await Task.Delay(schedule.GetNextOccurrence(t) - t, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            logger.LogTrace("Rates Update Service executed.");
        }

        private string DetermineTransactionType(TransactionObject transactionDetails, string publicKey)
        {
            bool isVinMatch = transactionDetails.Vin.Any(v => v.Addresses.Contains(publicKey));
            bool isVoutMatch = transactionDetails.Vout.Any(v => v.Addresses.Contains(publicKey));
            bool isMining = transactionDetails.Vin.Count == 1 && string.IsNullOrEmpty(transactionDetails.Vin.First().Txid);
            bool isStaking = transactionDetails.Vin.Sum(v => Convert.ToDecimal(v.Value)) > transactionDetails.Vout.Sum(v => Convert.ToDecimal(v.Value));

            if (isMining)
                return "Mining";
            if (isStaking)
                return "Staking";
            if (isVinMatch && isVoutMatch)
                return "Internal";
            if (isVinMatch)
                return "Outgoing";
            if (isVoutMatch)
                return "Incoming";

            return "Unknown";
        }

        private decimal CalculateTransactionAmount(TransactionObject transactionDetails, string publicKey, string transactionType)
        {
            decimal amount = 0;

            if (transactionType == "Incoming")
            {
                foreach (var vout in transactionDetails.Vout)
                {
                    if (vout.Addresses.Contains(publicKey))
                    {
                        amount += Convert.ToDecimal(vout.Value);
                    }
                }
            }
            else if (transactionType == "Outgoing")
            {
                foreach (var vin in transactionDetails.Vin)
                {
                    if (vin.Addresses.Contains(publicKey))
                    {
                        amount += Convert.ToDecimal(vin.Value);
                    }
                }

                // Subtracting the transaction fee from the total amount for outgoing transactions
                amount -= Convert.ToDecimal(transactionDetails.Fees);
            }

            return amount / 100000000; // TODO Convert from Satoshi
        }

    }
}