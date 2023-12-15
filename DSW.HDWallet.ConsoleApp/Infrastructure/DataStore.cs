using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Domain.Models;
using System.Text.Json;
using Wallet = DSW.HDWallet.ConsoleApp.Domain.Models.Wallet;

namespace DSW.HDWallet.ConsoleApp.Infrastructure
{
    namespace HDWalletConsoleApp.Infrastructure.DataStore
    {
        public class DataStore : IDataStore
        {
            private readonly string _filePath;
            private Dictionary<string, JsonElement> _data;

            public DataStore()
            {
                _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mnconsolestore.json");
                _data = LoadData();
            }

            public List<Wallet> Wallets
            {
                get => GetCollection<Wallet>(nameof(Wallets));
                set => _data[nameof(Wallets)] = JsonSerializer.SerializeToElement(value);
            }

            public List<CoinAddress> CoinAddresses
            {
                get => GetCollection<CoinAddress>(nameof(CoinAddresses));
                set => _data[nameof(CoinAddresses)] = JsonSerializer.SerializeToElement(value);
            }

            public List<Rate> Rates
            {
                get => GetCollection<Rate>(nameof(Rates));
                set => _data[nameof(Rates)] = JsonSerializer.SerializeToElement(value);
            }

            public List<Setting> Settings
            {
                get => GetCollection<Setting>(nameof(Settings));
                set => _data[nameof(Settings)] = JsonSerializer.SerializeToElement(value);
            }

            public List<WalletCoin> WalletCoins
            {
                get => GetCollection<WalletCoin>(nameof(WalletCoins));
                set => _data[nameof(WalletCoins)] = JsonSerializer.SerializeToElement(value);
            }

            public void SaveChanges()
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_data.ToDictionary(pair => pair.Key, pair => JsonDocument.Parse(pair.Value.GetRawText())), options);
                File.WriteAllText(_filePath, json);
            }

            private Dictionary<string, JsonElement> LoadData()
            {
                if (!File.Exists(_filePath)) return new Dictionary<string, JsonElement>();

                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                return data ?? new Dictionary<string, JsonElement>();
            }

            private List<T> GetCollection<T>(string key) where T : new()
            {
                if (!_data.TryGetValue(key, out var element) || element.ValueKind != JsonValueKind.Array)
                {
                    var newList = new List<T>();
                    _data[key] = JsonSerializer.SerializeToElement(newList);
                    return newList;
                }

                var deserializedList = JsonSerializer.Deserialize<List<T>>(element.GetRawText());
                if (deserializedList == null)
                {
                    deserializedList = new List<T>();
                    _data[key] = JsonSerializer.SerializeToElement(deserializedList);
                }
                return deserializedList;
            }

        }
    }
}
