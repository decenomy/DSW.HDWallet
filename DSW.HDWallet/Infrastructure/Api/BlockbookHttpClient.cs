using DSW.HDWallet.Domain.ApiObjects;
using System.Text;
using System.Text.Json;

namespace DSW.HDWallet.Infrastructure.Api
{
    public class BlockbookHttpClient : IBlockbookHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlockbookHttpClient(IHttpClientFactory httpClientFactory)
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

        public async Task<FeeResultObject> GetFeeEstimation(string coin, int blockNumber)
        {
            string endpoint = $"/api/v1/estimatefee/{blockNumber}";
            var apiUrl = BuildApiUrl(coin, endpoint);

            return await SendGetRequest<FeeResultObject>(apiUrl);
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

        public async Task<XpubObject> GetXpub(string coin, string xpub, int page = 1, int pageSize = 1000)
        {
            string endpoint = $"/api/v2/xpub/{xpub}?page={page}&pageSize={pageSize}";
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

        public async Task<TransactionSendResponse> SendTransaction(string ticker, string rawTransaction)
        {
            var content = new StringContent(rawTransaction, Encoding.UTF8, "text/plain");
            string endpoint = $"/api/v2/sendtx/";
            var apiUrl = BuildApiUrl(ticker, endpoint);

            return await SendPostRequest<TransactionSendResponse>(apiUrl, content);
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

        private async Task<T> SendPostRequest<T>(string apiUrl, HttpContent content)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsync(apiUrl, content);

                // TODO add to logs
                var a = response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    return JsonSerializer.Deserialize<T>(responseBody, options)!;
                }
                else
                {
                    throw new Exception($"Error in API POST request: {response.Content} StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in API POST request: {ex.Message}");
            }
        }

    }
}
