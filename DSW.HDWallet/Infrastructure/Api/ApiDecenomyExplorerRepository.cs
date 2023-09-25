using DSW.HDWallet.Domain.ApiObjects;
using Newtonsoft.Json;

namespace DSW.HDWallet.Infrastructure.Api
{
    public class ApiDecenomyExplorerRepository : IApiDecenomyExplorerRepository
    {
        public async Task<AddressObject> GetAddressAsync(string coin, string address)
        {
            string endpoint = $"/api/v2/address/{address}";         

            try
            {
                // TO DO Criar Factory / Ingles / DeserializeObject nativo do .NET Core / Exception

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(getApiUrl(endpoint, coin));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<AddressObject>(responseBody);
                }
                else
                {
                    throw new Exception($"Erro na solicitação à API: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro na solicitação à API: {ex.Message}");
            }
        }

        public async Task<TransactionObject> GetTransactionAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx/{txid}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(getApiUrl(endpoint, coin));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TransactionObject>(responseBody);
                }
                else
                {
                    throw new Exception($"Erro na solicitação à API: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro na solicitação à API: {ex.Message}");
            }
        }

        public async Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid)
        {
            string endpoint = $"/api/v2/tx-specific/{txid}";

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(getApiUrl(endpoint, coin));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TransactionSpecificObject>(responseBody);
                }
                else
                {
                    throw new Exception($"Erro na solicitação à API: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro na solicitação à API: {ex.Message}");
            }
        }

        private string getApiUrl(string endpoint, string coin)
        {         
            return $"https://{coin.ToLower()}.decenomy.net{endpoint}";
        }
    }
}
