using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Infrastructure.Coins;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class CoinRepository : ICoinRepository
    {
        private readonly Dictionary<string, ICoinExtension> coins = new();

        public CoinRepository()
        {
            coins[Azzure.Instance.Ticker] = Azzure.Instance as ICoinExtension;
            coins[Beacon.Instance.Ticker] = Beacon.Instance as ICoinExtension;
            coins[Birake.Instance.Ticker] = Birake.Instance as ICoinExtension;
            coins[CryptoFlow.Instance.Ticker] = CryptoFlow.Instance as ICoinExtension;
            coins[CryptoSaga.Instance.Ticker] = CryptoSaga.Instance as ICoinExtension;
            coins[DashDiamond.Instance.Ticker] = DashDiamond.Instance as ICoinExtension;
            coins[EskaCoin.Instance.Ticker] = EskaCoin.Instance as ICoinExtension;
            coins[Flits.Instance.Ticker] = Flits.Instance as ICoinExtension;
            coins[Jackpot.Instance.Ticker] = Jackpot.Instance as ICoinExtension;
            coins[Kyanite.Instance.Ticker] = Kyanite.Instance as ICoinExtension;
            coins[MobilityCoin.Instance.Ticker] = MobilityCoin.Instance as ICoinExtension;
            coins[Monk.Instance.Ticker] = Monk.Instance as ICoinExtension;
            coins[OneWorldCoin.Instance.Ticker] = OneWorldCoin.Instance as ICoinExtension;
            coins[Peony.Instance.Ticker] = Peony.Instance as ICoinExtension;
            coins[Sapphire.Instance.Ticker] = Sapphire.Instance as ICoinExtension;
            coins[Suvereno.Instance.Ticker] = Suvereno.Instance as ICoinExtension;
            coins[UltraClear.Instance.Ticker] = UltraClear.Instance as ICoinExtension;

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
