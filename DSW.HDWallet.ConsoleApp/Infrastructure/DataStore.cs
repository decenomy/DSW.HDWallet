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

            public List<Wallet> Wallets => _data.GetValueOrDefault(nameof(Wallets)) as List<Wallet> ?? new List<Wallet>();
            public List<CoinAddress> CoinAddresses => _data.GetValueOrDefault(nameof(CoinAddresses)) as List<CoinAddress> ?? new List<CoinAddress>();
            public List<Rate> Rates => _data.GetValueOrDefault(nameof(Rates)) as List<Rate> ?? new List<Rate>();
            public List<WalletCoin> WalletCoins => _data.GetValueOrDefault(nameof(WalletCoins)) as List<WalletCoin> ?? new List<WalletCoin>();
            public List<Setting> Settings => _data.GetValueOrDefault(nameof(Settings)) as List<Setting> ?? new List<Setting>();

            public void SaveChanges()
            {
                var json = JsonSerializer.Serialize(_data);
                File.WriteAllText(_filePath, json);
            }

            private Dictionary<string, object> LoadData()
            {
                if (!File.Exists(_filePath)) return new Dictionary<string, object>();

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
        }
    }
}
