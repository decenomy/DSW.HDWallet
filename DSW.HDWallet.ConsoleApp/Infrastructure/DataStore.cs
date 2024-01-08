using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Interfaces;
using System.Text.Json;

namespace HDWalletConsoleApp.Infrastructure.DataStore
{
    public class DataStore : IStorage, ISecureStorage
    {
        private readonly string _filePath;
        private Dictionary<string, JsonElement> _data;

        public List<Seed> Seeds { get; private set; }
        public List<CoinAddress> CoinAddresses { get; private set; }
        public List<Rate> Rates { get; private set; }
        public List<Wallet> Wallets { get; private set; }
        public DataStore()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mnconsolestore.json");
            _data = LoadData();

            Seeds = GetCollection<Seed>(nameof(Seeds));
            CoinAddresses = GetCollection<CoinAddress>(nameof(CoinAddresses));
            Rates = GetCollection<Rate>(nameof(Rates));
            Wallets = GetCollection<Wallet>(nameof(Wallets));

        }

        public void SaveChanges()
        {
            UpdateData(nameof(Seeds), Seeds);
            UpdateData(nameof(CoinAddresses), CoinAddresses);
            UpdateData(nameof(Rates), Rates);
            UpdateData(nameof(Wallets), Wallets);

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

        public void AddCoinAddress(CoinAddress coinAddress)
        {
            if (CoinAddresses.Any(ca => ca.Address == coinAddress.Address))
            {
                // Address already exists
                return;
            }

            CoinAddresses.Add(coinAddress);
            SaveChanges();
        }

        public void IncrementCoinIndex(string ticker)
        {
            var wallet = Wallets.FirstOrDefault(w => w.Ticker == ticker);
            if (wallet != null)
            {
                wallet.CoinIndex++;
                SaveChanges();
            }
        }

        public CoinAddress GetAddressByAddress(string address)
        {
            return CoinAddresses.FirstOrDefault(ca => ca.Address == address)!;
        }

        public void UpdateAddressUsed(CoinAddress coinAddress)
        {
            var existingAddress = CoinAddresses.FirstOrDefault(ca => ca.Address == coinAddress.Address);
            if (existingAddress != null)
            {
                existingAddress.IsUsed = true;
                SaveChanges();
            }
        }

        private void UpdateData<T>(string key, List<T> collection)
        {
            _data[key] = JsonSerializer.SerializeToElement(collection);
        }

        public void AddWallet(Seed seed)
        {
            Seeds.Add(seed);
            SaveChanges();
        }

        public void DeleteAllData()
        {
            Seeds.Clear();
            CoinAddresses.Clear();
            Rates.Clear();
            Wallets.Clear();
            SaveChanges();
        }

        public bool HasSeed()
        {
            return Seeds.Any();
        }

        public string GetMnemonic()
        {
            return Seeds.First().Mnemonic ?? "No mnemonic available";
        }

        public bool AddCoin(Wallet wallet)
        {
            Wallets.Add(wallet); 
            SaveChanges();       
            return true;
        }

        public bool AddAddress(CoinAddress coinAddress)
        {
            CoinAddresses.Add(coinAddress);
            SaveChanges();
            return true;
        }

        public Task<IEnumerable<Wallet>> GetAllWallets()
        {
            return Task.FromResult<IEnumerable<Wallet>>(Wallets);
        }

        public CoinAddress? GetUnusedAddress(string ticker)
        {
            return CoinAddresses.FirstOrDefault(ca => ca.Ticker == ticker && !ca.IsUsed);
        }

        public Wallet? GetWallet(string ticker)
        {
            return Wallets.FirstOrDefault(w => w.Ticker == ticker);
        }

        public async Task SaveRates(Rate rate)
        {
            var existingRate = Rates.FirstOrDefault(r => r.TickerFrom == rate.TickerFrom && r.TickerTo == rate.TickerTo);
            if (existingRate != null)
            {
                existingRate.RateValue = rate.RateValue; // Update the existing rate
            }
            else
            {
                Rates.Add(rate);
            }

            SaveChanges();
            await Task.CompletedTask;
        }

        public async Task SaveBalance(Wallet coin)
        {
            var walletCoin = Wallets.FirstOrDefault(w => w.Ticker == coin.Ticker && w.PublicKey == coin.PublicKey);
            if (walletCoin != null)
            {
                walletCoin.Balance = coin.Balance;
                walletCoin.UnconfirmedBalance = coin.UnconfirmedBalance;
                SaveChanges();
            }

            await Task.CompletedTask; 
        }

        //public long GetBalanceByTicker(string ticker)
        //{
        //    var wallet = Wallets.FirstOrDefault(w => w.Ticker == ticker);
        //    return wallet?.Balance ?? 0;
        //}

    }
}

