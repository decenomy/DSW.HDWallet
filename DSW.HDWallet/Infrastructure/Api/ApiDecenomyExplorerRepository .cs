using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using System.Text.Json;

namespace DSW.HDWallet.Infrastructure.Api
{
    public class ApiDecenomyExplorerRepository : IApiDecenomyExplorerRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiDecenomyExplorerRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<AddressObject> GetAddressAsync(string coin, string address)
        {
            string endpoint = $"/api/v2/address/{address}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<AddressObject>(apiUrl);
        }

        public async Task<TransactionObject> GetTransactionAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx/{txid}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<TransactionObject>(apiUrl);
        }

        public async Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx-specific/{txid}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<TransactionSpecificObject>(apiUrl);
        }

        public async Task<BlockHashObject> GetBlockHash(string coin, string blockHeight)
        {
            string endpoint = $"/api/v2/block-index/{blockHeight}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<BlockHashObject>(apiUrl);
        }

        public async Task<XpubObject> GetXpub(string coin, string xpub)
        {
            string endpoint = $"/api/v2/xpub/{xpub}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<XpubObject>(apiUrl);
        }

        public async Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed)
        {            
            var queryParams = new Dictionary<string, string>
            {
                {"confirmed", confirmed.ToString()}
            };            

            string endpoint = $"/api/v2/utxo/{address}";
            var apiUrl = BuildApiUrl(coin, endpoint, queryParams);

            return await SendGetRequest<UtxoObject[]>(apiUrl);
        }

        private string BuildApiUrl(string coin, string endpoint, Dictionary<string, string>? queryParams = null)
        {
            var uriBuilder = new UriBuilder($"https://{coin.ToLower()}.decenomy.net{endpoint}");

            if (queryParams != null && queryParams.Count > 0)
            {
                var query = new FormUrlEncodedContent(queryParams);
                uriBuilder.Query = query.ReadAsStringAsync().Result;
            }

            return uriBuilder.ToString();
        }

        private async Task<T> SendGetRequest<T>(string apiUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    return JsonSerializer.Deserialize<T>(responseBody, options)!;
                }
                else
                {
                    throw new Exception(message: $"Error in API request: {response.Content} StatusCode: {response.StatusCode} ");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in API request: {ex.Message}");
            }
        }


    }
}
