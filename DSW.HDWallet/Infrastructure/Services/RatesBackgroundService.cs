using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class RatesBackgroundService : BaseBackgroundService<RatesBackgroundService>
    {
        private readonly IRatesService ratesService;

        public RatesBackgroundService(
            ILogger<RatesBackgroundService> logger,
            IRatesService ratesService
        ) : base(logger, "0 */5 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.ratesService = ratesService;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Rates Update Service executing.");

            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    await ratesService.UpdateRatesAsync();

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