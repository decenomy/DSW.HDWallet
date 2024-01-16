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
                        var xpub = await blockbookHttpClient.GetXpub(wallet.Ticker!, wallet.PublicKey!);

                        foreach (var txid in xpub.Txids!)
                        {
                            var existingTransaction = await storage.GetTransactionByTxId(txid);
                            if (existingTransaction == null)
                            {
                                var transactionDetails = await blockbookHttpClient.GetTransactionAsync(wallet.Ticker!, txid);

                                var transactionRecord = new TransactionRecord
                                {
                                    TxId = transactionDetails.Txid,
                                    Ticker = wallet.Ticker,
                                    // TODO Save more info
                                };

                                await storage.AddTransaction(transactionRecord);
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
    }
}