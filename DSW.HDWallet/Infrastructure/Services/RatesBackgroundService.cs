using DSW.HDWallet.Application;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using Newtonsoft.Json;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class RatesBackgroundService : BaseBackgroundService<RatesBackgroundService>
    {
        private readonly IRatesUpdateService ratesUpdateService;

        public RatesBackgroundService(
            ILogger<RatesBackgroundService> logger,
            IRatesUpdateService ratesUpdateService
        ) : base(logger, "0 */5 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.ratesUpdateService = ratesUpdateService;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Rates Update Service executing.");

            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    await ratesUpdateService.UpdateRatesAsync();

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