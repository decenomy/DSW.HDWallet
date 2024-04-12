using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class BalancesBackgroundService : BaseBackgroundService<BalancesBackgroundService>
    {
        private readonly IBalanceService balanceService;

        public BalancesBackgroundService(
            ILogger<BalancesBackgroundService> logger,
            IBalanceService balanceService
        ) : base(logger, "0 */5 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.balanceService = balanceService;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Balance Update Service executing.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await balanceService.UpdateAllBalancesAsync();

                    var t = DateTime.Now;
                    await Task.Delay(schedule.GetNextOccurrence(t) - t, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            logger.LogTrace("Balance Update Service executed.");
        }
    }
}
