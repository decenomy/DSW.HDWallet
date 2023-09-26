using DSW.HDWallet.Domain.ApiObjects;
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
            return await SendGetRequest<AddressObject>(coin, endpoint);
        }

        public async Task<TransactionObject> GetTransactionAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx/{txid}";
            return await SendGetRequest<TransactionObject>(coin, endpoint);
        }

        public async Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx-specific/{txid}";
            return await SendGetRequest<TransactionSpecificObject>(coin, endpoint);
        }

        private async Task<T> SendGetRequest<T>(string coin, string endpoint)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = $"https://{coin.ToLower()}.decenomy.net{endpoint}";

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
