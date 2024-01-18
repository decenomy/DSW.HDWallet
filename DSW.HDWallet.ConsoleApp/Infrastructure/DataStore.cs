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
        public List<TransactionRecord> TransactionRecords { get; private set; }

        public DataStore()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mnconsolestore.json");
            _data = LoadData();

            Seeds = GetCollection<Seed>(nameof(Seeds));
            CoinAddresses = GetCollection<CoinAddress>(nameof(CoinAddresses));
            Rates = GetCollection<Rate>(nameof(Rates));
            Wallets = GetCollection<Wallet>(nameof(Wallets));
            TransactionRecords = GetCollection<TransactionRecord>(nameof(TransactionRecords));
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
        private void UpdateData<T>(string key, List<T> collection)
        {
            _data[key] = JsonSerializer.SerializeToElement(collection);
        }
        // IStorage Methods
        public Task AddWallet(Seed seed)
        {
            Seeds.Add(seed);
            SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteAllData()
        {
            Seeds.Clear();
            CoinAddresses.Clear();
            Rates.Clear();
            Wallets.Clear();
            SaveChanges();
            return Task.CompletedTask;
        }

        public Task<bool> AddCoin(Wallet wallet)
        {
            Wallets.Add(wallet);
            SaveChanges();
            return Task.FromResult(true);
        }

        public Task<bool> AddAddress(CoinAddress coinAddress)
        {
            CoinAddresses.Add(coinAddress);
            SaveChanges();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Wallet>> GetAllWallets()
        {
            return Task.FromResult<IEnumerable<Wallet>>(Wallets);
        }

        public Task<IEnumerable<Rate>> GetAllRates()
        {
            return Task.FromResult<IEnumerable<Rate>>(Rates);
        }

        public Task<CoinAddress?> GetUnusedAddress(string ticker)
        {
            var unusedAddress = CoinAddresses.FirstOrDefault(ca => ca.Ticker == ticker && !ca.IsUsed);
            return Task.FromResult<CoinAddress?>(unusedAddress);
        }

        public Task<Wallet?> GetWallet(string ticker)
        {
            var wallet = Wallets.FirstOrDefault(w => w.Ticker == ticker);
            return Task.FromResult<Wallet?>(wallet);
        }

        public Task SaveRates(Rate rate)
        {
            var existingRate = Rates.FirstOrDefault(r => r.TickerFrom == rate.TickerFrom && r.TickerTo == rate.TickerTo);
            if (existingRate != null)
            {
                existingRate.RateValue = rate.RateValue;
            }
            else
            {
                Rates.Add(rate);
            }
            SaveChanges();
            return Task.CompletedTask;
        }

        public Task SaveBalance(Wallet coin)
        {
            var walletCoin = Wallets.FirstOrDefault(w => w.Ticker == coin.Ticker && w.PublicKey == coin.PublicKey);
            if (walletCoin != null)
            {
                walletCoin.Balance = coin.Balance;
                walletCoin.UnconfirmedBalance = coin.UnconfirmedBalance;
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task AddCoinAddress(CoinAddress coinAddress)
        {
            if (!CoinAddresses.Any(ca => ca.Address == coinAddress.Address))
            {
                CoinAddresses.Add(coinAddress);
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task IncrementCoinIndex(string ticker)
        {
            var wallet = Wallets.FirstOrDefault(w => w.Ticker == ticker);
            if (wallet != null)
            {
                wallet.CoinIndex++;
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task<CoinAddress> GetAddressByAddress(string address)
        {
            var addressResult = CoinAddresses.FirstOrDefault(ca => ca.Address == address)!;
            return Task.FromResult(addressResult);
        }

        public Task UpdateAddressUsed(CoinAddress coinAddress)
        {
            var existingAddress = CoinAddresses.FirstOrDefault(ca => ca.Address == coinAddress.Address);
            if (existingAddress != null)
            {
                existingAddress.IsUsed = true;
                SaveChanges();
            }
            return Task.CompletedTask;
        }

        // ISecureStorage Methods
        public Task<bool> HasSeed()
        {
            return Task.FromResult(Seeds.Any());
        }

        public Task<string> GetMnemonic()
        {
            string mnemonic = Seeds.FirstOrDefault()?.Mnemonic ?? "No mnemonic available";
            return Task.FromResult(mnemonic);
        }

        public Task AddTransaction(TransactionRecord transaction)
        {
            TransactionRecords.Add(transaction);
            SaveChanges();
            return Task.CompletedTask;
        }

        public Task<TransactionRecord?> GetTransactionByTxId(string txid)
        {
            var transaction = TransactionRecords.FirstOrDefault(t => t.TxId == txid);
            return Task.FromResult<TransactionRecord?>(transaction);
        }

        public Task<IEnumerable<CoinAddress>> GetAddressesByTicker(string ticker)
        {
            var addresses = CoinAddresses.Where(ca => ca.Ticker == ticker);
            return Task.FromResult<IEnumerable<CoinAddress>>(addresses);
        }
    }
}
