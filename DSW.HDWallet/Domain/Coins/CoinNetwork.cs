using DSW.HDWallet.Infrastructure.Coins;
using NBitcoin;

namespace DSW.HDWallet.Domain.Coins
{
    public static class CoinNetwork
    {
        public static Network GetMainnet(CoinType coinType)
        {
            return coinType switch
            {
                CoinType.AZR => Azzure.Instance.Mainnet,
                CoinType.BECN => Beacon.Instance.Mainnet,
                CoinType.BIR => Birake.Instance.Mainnet,
                CoinType.CFL => CryptoFlow.Instance.Mainnet,
                CoinType.SAGA => CryptoSaga.Instance.Mainnet,
                CoinType.DASHD => DashDiamond.Instance.Mainnet,
                CoinType.ESK => EskaCoin.Instance.Mainnet,
                CoinType.FLS => Flits.Instance.Mainnet,
                CoinType._777 => Jackpot.Instance.Mainnet,
                CoinType.KYAN => Kyanite.Instance.Mainnet,
                CoinType.MOBIC => MobilityCoin.Instance.Mainnet,
                CoinType.MONK => Monk.Instance.Mainnet,
                CoinType.OWO => OneWorldCoin.Instance.Mainnet,
                CoinType.PNY => Peony.Instance.Mainnet,
                CoinType.SAPP => Sapphire.Instance.Mainnet,
                CoinType.SUV => Suvereno.Instance.Mainnet,
                CoinType.UCR => UltraClear.Instance.Mainnet,
                _ => throw new ArgumentOutOfRangeException(nameof(coinType), coinType, "Unknown coin type"),
            };
        }
    }
}
