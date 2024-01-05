using DSW.HDWallet.Application;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class BalanceUpdateService : BaseBackgroundService<BalanceUpdateService>
    {
        private readonly IStorage storage;
        IWalletService walletService;
        public BalanceUpdateService(
            ILogger<BalanceUpdateService> logger,
            IStorage storage,
            IWalletService walletService
        ) : base(logger, "0 */5 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.storage = storage;
            this.walletService = walletService;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Balance Update Service executing.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                { 
                    var walletCoins = await storage.GetAllWallets();

                    foreach (var coin in walletCoins)
                    {
                        if (coin.Ticker != null && coin.PublicKey != null)
                        {
                            var balance = await walletService.GetXpub(coin.Ticker, coin.PublicKey, 1, 1);

                            if (long.TryParse(balance.Balance, out long balanceValue))
                            {
                                decimal realBalance = SatoshiConverter.FromSatoshi(balanceValue);
                                coin.Balance = SatoshiConverter.ToSubSatoshi(realBalance);
                                await storage.SaveBalance(coin);
                            }
                            else
                            {
                                logger.LogError("Error parsing balance.");
                            }
                        }
                        else
                        {
                            logger.LogError("Existing data error on updating balance.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }

                var t = DateTime.Now;
                await Task.Delay(schedule.GetNextOccurrence(t) - t, cancellationToken);
            }
            logger.LogTrace("Balance Update Service executed.");
        }

    }
}