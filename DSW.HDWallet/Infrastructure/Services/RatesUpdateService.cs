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
    public class RatesUpdateService : IRatesUpdateService
    {
        private readonly ICoinManager coinManager;
        private readonly ICoinGeckoService coingeckoService;
        private readonly IStorage storage;

        public RatesUpdateService(ICoinManager coinManager, ICoinGeckoService coingeckoService, IStorage storage)
        {
            this.coinManager = coinManager;
            this.coingeckoService = coingeckoService;
            this.storage = storage;
        }

        public async Task UpdateRatesAsync()
        {
            Dictionary<string, string> tickerMapping = coinManager.GetCoinGeckoIds();
            List<string> currencies = CurrencyVS.GetCurrencyTickers();
            List<string> coinGeckoIds = new(tickerMapping.Values);

            // Fetch rates from Coingecko
            string jsonRates = await coingeckoService.GetRatesAsync(coinGeckoIds, currencies);

            var ratesData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(jsonRates);

            if (ratesData != null)
            {
                foreach (var rateData in ratesData)
                {
                    // Find the internal ticker symbol that corresponds to the CoinGecko ID
                    var tickerFrom = tickerMapping.FirstOrDefault(x => x.Value.Equals(rateData.Key, StringComparison.OrdinalIgnoreCase)).Key;
                    if (string.IsNullOrEmpty(tickerFrom))
                    {
                        continue; // Skip if no matching ticker symbol is found
                    }

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

        }
    }
}