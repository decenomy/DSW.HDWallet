using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

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
                    //Logic here
                    var wallets = await storage.GetAllWallets();

                    foreach (var wallet in wallets)
                    {
                        var xpub = await blockbookHttpClient.GetXpub(wallet.Ticker!, wallet.PublicKey!);

                        foreach (var txid in xpub.Txids!)
                        {
                            if (true)//check if txID is already in database
                            {
                                var transactionDetails = await blockbookHttpClient.GetTransactionAsync(wallet.Ticker!, txid);

                            }
                            //if (!Database.Contains(txid)) // Assuming a method to check if the txid is in your database
                            //{
                            //    var transactionDetails = await blockbookHttpClient.GetTransactionAsync(wallet.Ticker, txid);

                            //    // Save transactionDetails to your database
                            //    Database.SaveTransaction(transactionDetails); // Assuming a method to save transaction
                            //}
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