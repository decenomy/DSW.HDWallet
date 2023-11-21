using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Infrastructure.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class CoinRepository : ICoinRepository
    {
        private readonly Dictionary<string, ICoinExtension> coins = new();

        public CoinRepository()
        {
            coins[Azzure.Instance.Ticker] = Azzure.Instance as ICoinExtension;
            // ...
            coins[Kyanite.Instance.Ticker] = Kyanite.Instance as ICoinExtension;
            coins[Kyanite.InstanceTestnet.Ticker] = Kyanite.InstanceTestnet as ICoinExtension;
        }

        public Network GetNetwork(string ticker)
        {
            var coin = coins[ticker];
            var networkSetBase = coin as NetworkSetBase;

            return coin.IsTestNet ? networkSetBase!.Testnet : networkSetBase!.Mainnet;
        }

        public IEnumerable<ICoinExtension> Coins => coins.Values;

        public ICoinExtension GetCoin(string ticker) => coins[ticker];
    }
}
