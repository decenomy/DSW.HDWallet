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
            private Dictionary<string, object> _data;

            public DataStore()
            {
                _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mnconsolestore.json");
                _data = LoadData();
            }

            public List<Wallet> Wallets
            {
                get => GetCollection<Wallet>(nameof(Wallets));
                set => _data[nameof(Wallets)] = value;
            }

            public List<CoinAddress> CoinAddresses
            {
                get => GetCollection<CoinAddress>(nameof(CoinAddresses));
                set => _data[nameof(CoinAddresses)] = value;
            }

            public List<Rate> Rates
            {
                get => GetCollection<Rate>(nameof(Rates));
                set => _data[nameof(Rates)] = value;
            }

            public List<WalletCoin> WalletCoins
            {
                get => GetCollection<WalletCoin>(nameof(WalletCoins));
                set => _data[nameof(WalletCoins)] = value;
            }

            public List<Setting> Settings
            {
                get => GetCollection<Setting>(nameof(Settings));
                set => _data[nameof(Settings)] = value;
            }

            public void SaveChanges()
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_data, options);
                File.WriteAllText(_filePath, json);
            }

            private Dictionary<string, object> LoadData()
            {
                if (!File.Exists(_filePath)) return new Dictionary<string, object>();

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }

            private List<T> GetCollection<T>(string key) where T : new()
            {
                if (!_data.TryGetValue(key, out var collection))
                {
                    collection = new List<T>();
                    _data[key] = collection;
                }

                if (collection is List<T> typedCollection)
                {
                    return typedCollection;
                }
                else
                {
                    var newCollection = new List<T>();
                    _data[key] = newCollection;
                    return newCollection;
                }
            }
        }
    }
}
