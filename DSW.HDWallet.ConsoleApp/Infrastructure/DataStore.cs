using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Domain.Models;
using System.Text.Json;
using Wallet = DSW.HDWallet.ConsoleApp.Domain.Models.Wallet;

namespace HDWalletConsoleApp.Infrastructure.DataStore
{
    public class DataStore : IDataStore
    {
        private readonly string _filePath;
        private Dictionary<string, JsonElement> _data;

        public List<Wallet> Wallets { get; private set; }
        public List<CoinAddress> CoinAddresses { get; private set; }
        public List<Rate> Rates { get; private set; }
        public List<WalletCoin> WalletCoins { get; private set; }
        public DataStore()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mnconsolestore.json");
            _data = LoadData();

            Wallets = GetCollection<Wallet>(nameof(Wallets));
            CoinAddresses = GetCollection<CoinAddress>(nameof(CoinAddresses));
            Rates = GetCollection<Rate>(nameof(Rates));
            WalletCoins = GetCollection<WalletCoin>(nameof(WalletCoins));

        }

        public void SaveChanges()
        {
            UpdateData(nameof(Wallets), Wallets);
            UpdateData(nameof(CoinAddresses), CoinAddresses);
            UpdateData(nameof(Rates), Rates);
            UpdateData(nameof(WalletCoins), WalletCoins);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_data, options);
            File.WriteAllText(_filePath, json);
        }

        private Dictionary<string, JsonElement> LoadData()
        {
            if (!File.Exists(_filePath)) return new Dictionary<string, JsonElement>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
        }

        private List<T> GetCollection<T>(string key) where T : new()
        {
            if (_data.TryGetValue(key, out var element) && element.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(element.GetRawText()) ?? new List<T>();
            }
            return new List<T>();
        }

        private void UpdateData<T>(string key, List<T> collection)
        {
            _data[key] = JsonSerializer.SerializeToElement(collection);
        }

        public void AddWallet(Wallet wallet)
        {
            Wallets.Add(wallet);
            SaveChanges();
        }

        public void DeleteAllData()
        {
            Wallets.Clear();
            CoinAddresses.Clear();
            Rates.Clear();
            WalletCoins.Clear();
            SaveChanges();
        }

        public bool HasWallets()
        {
            return Wallets.Any();
        }
    }
}

