using DSW.HDWallet.Infrastructure.Interfaces;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class CoingeckoService : ICoinGeckoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CoingeckoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetRatesAsync(IEnumerable<string> tickers, IEnumerable<string> currencies)
        {
            string currenciesJoined = string.Join(",", currencies);
            string tickersJoined = string.Join(",", tickers);

            var client = _httpClientFactory.CreateClient("coingeckoapi");
            var response = await client.GetAsync($"price?vs_currencies={currenciesJoined}&ids={tickersJoined}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}