using DSW.HDWallet.Domain.ApiObjects;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using WebSocketSharp;

namespace DSW.HDWallet.Infrastructure.Api
{
    public class BlockbookHttpClient : IBlockbookHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BlockbookHttpClient> logger;

        public BlockbookHttpClient(IHttpClientFactory httpClientFactory, ILogger<BlockbookHttpClient> logger)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger;
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
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<T>(responseBody, options)!;
                }
                else
                {
                    logger.LogError($"Error in API GET request: {response.Content} StatusCode: {response.StatusCode}");
                    throw new Exception($"Error in API GET request: {response.Content} StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in API GET request: {ex.Message}");
                throw;
            }
        }

        private async Task<T> SendPostRequest<T>(string apiUrl, HttpContent content)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<T>(responseBody, options)!;
                }
                else
                {
                    logger.LogError($"Error in API POST request: {response.Content} StatusCode: {response.StatusCode}");
                    throw new Exception($"Error in API POST request: {response.Content} StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in API POST request: {ex.Message}");
                throw;
            }
        }

    }
}
