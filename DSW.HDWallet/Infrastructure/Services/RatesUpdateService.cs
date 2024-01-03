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
    public class RatesUpdateService : BaseBackgroundService<RatesUpdateService>
    {
        private readonly ICoinGeckoService coingeckoService;
        private readonly IWalletService walletService;
        private readonly ICoinManager coinManager;
        private readonly IStorage storage;
        private readonly ICoinRepository coinRepository;

        public RatesUpdateService(
            ILogger<RatesUpdateService> logger,
            ICoinGeckoService coingeckoService,
            ICoinManager coinManager,
            IStorage storage,
            IWalletService walletService,
            ICoinRepository coinRepository
        ) : base(logger, "0 */5 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.coingeckoService = coingeckoService;
            this.coinManager = coinManager;
            this.storage = storage;
            this.walletService = walletService;
            this.coinRepository = coinRepository;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Rates Update Service executing.");

            try
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    Dictionary<string, string> tickerMapping = coinManager.GetCoinGeckoIds();
                    List<string> currencies = CurrencyVS.GetCurrencyTickers();
                    List<string> tickers = new(tickerMapping.Values);

                    // Fetch rates from Coingecko
                    string jsonRates = await coingeckoService.GetRatesAsync(tickers, currencies);

                    var ratesData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(jsonRates);

                    if (ratesData != null)
                    {
                        foreach (var rateData in ratesData)
                        {
                            var tickerFrom = rateData.Key;
                            foreach (var currencyRate in rateData.Value)
                            {
                                var tickerTo = currencyRate.Key;
                                var rateValue = currencyRate.Value;

                                long satoshiValue = SatoshiConverter.ToSubSatoshi(rateValue);

                                var rate = new Rate
                                {
                                    TickerFrom = tickerFrom,
                                    TickerTo = tickerTo,
                                    RateValue = satoshiValue
                                };

                                await storage.SaveRates(rate);
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