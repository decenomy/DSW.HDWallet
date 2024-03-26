using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class RatesService : IRatesService
    {
        private readonly ICoinManager coinManager;
        private readonly ICoinGeckoService coingeckoService;
        private readonly IStorage storage;

        public RatesService(ICoinManager coinManager, 
            ICoinGeckoService coingeckoService, 
            IStorage storage)
        {
            this.coinManager = coinManager;
            this.coingeckoService = coingeckoService;
            this.storage = storage;
        }

        public async Task UpdateRatesAsync()
        {
            Dictionary<string, string> tickerMapping = await coinManager.GetCoinGeckoIds();
            List<string> currencies = CurrencyVS.GetCurrencyTickers();
            List<string> coinGeckoIds = new(tickerMapping.Values.Distinct().ToList());

            string jsonRates = await coingeckoService.GetRatesAsync(coinGeckoIds, currencies);
            var ratesData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(jsonRates);

            if (ratesData != null)
            {
                foreach (var rateData in ratesData)
                {
                    var matchingTickers = tickerMapping.Where(x => x.Value.Equals(rateData.Key, StringComparison.OrdinalIgnoreCase)).Select(x => x.Key).ToList();
                    foreach (var tickerFrom in matchingTickers)
                    {
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
}
