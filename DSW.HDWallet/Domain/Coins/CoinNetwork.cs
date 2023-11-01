using DSW.HDWallet.Infrastructure.Coins;
using NBitcoin;

namespace DSW.HDWallet.Domain.Coins
{
    public static class CoinNetwork
    {
        public static Network GetMainnet(CoinType coinType, bool isNetworkTest = false)
        {
            return coinType switch
            {
                CoinType.AZR => isNetworkTest ? Azzure.Instance.Testnet : Azzure.Instance.Mainnet,
                CoinType.BECN => isNetworkTest ? Beacon.Instance.Testnet : Beacon.Instance.Mainnet,
                CoinType.BIR => isNetworkTest ? Birake.Instance.Testnet : Birake.Instance.Mainnet,
                CoinType.CFL => isNetworkTest ? CryptoFlow.Instance.Testnet : CryptoFlow.Instance.Mainnet,
                CoinType.SAGA => isNetworkTest ? CryptoSaga.Instance.Testnet : CryptoSaga.Instance.Mainnet,
                CoinType.DASHD => isNetworkTest ? DashDiamond.Instance.Testnet : DashDiamond.Instance.Mainnet,
                CoinType.ESK => isNetworkTest ? EskaCoin.Instance.Testnet : EskaCoin.Instance.Mainnet,
                CoinType.FLS => isNetworkTest ? Flits.Instance.Testnet : Flits.Instance.Mainnet,
                CoinType._777 => isNetworkTest ? Jackpot.Instance.Testnet : Jackpot.Instance.Mainnet,
                CoinType.KYAN => isNetworkTest ? Kyanite.Instance.Testnet : Kyanite.Instance.Mainnet,
                CoinType.MOBIC => isNetworkTest ? MobilityCoin.Instance.Testnet : MobilityCoin.Instance.Mainnet,
                CoinType.MONK => isNetworkTest ? Monk.Instance.Testnet : Monk.Instance.Mainnet,
                CoinType.OWO => isNetworkTest ? OneWorldCoin.Instance.Testnet : OneWorldCoin.Instance.Mainnet,
                CoinType.PNY => isNetworkTest ? Peony.Instance.Testnet : Peony.Instance.Mainnet,
                CoinType.SAPP => isNetworkTest ? Sapphire.Instance.Testnet : Sapphire.Instance.Mainnet,
                CoinType.SUV => isNetworkTest ? Suvereno.Instance.Testnet : Suvereno.Instance.Mainnet,
                CoinType.UCR => isNetworkTest ? UltraClear.Instance.Testnet : UltraClear.Instance.Mainnet,
                // Test
                CoinType.TKYAN => Kyanite.Instance.Testnet,
                _ => throw new ArgumentOutOfRangeException(nameof(coinType), coinType, "Unknown coin type"),
            };
        }

        public static Network GetNetwork(string coinType)
        {
            return coinType switch
            {
                "AZR" => Azzure.Instance.Mainnet,
                "BECN" => Beacon.Instance.Mainnet,
                "BIR" => Birake.Instance.Mainnet,
                "CFL" => CryptoFlow.Instance.Mainnet,
                "SAGA" => CryptoSaga.Instance.Mainnet,
                "DASHD" => DashDiamond.Instance.Mainnet,
                "ESK" => EskaCoin.Instance.Mainnet,
                "FLS" => Flits.Instance.Mainnet,
                "777" => Jackpot.Instance.Mainnet,
                "KYAN" => Kyanite.Instance.Mainnet,
                "MOBIC" => MobilityCoin.Instance.Mainnet,
                "MONK" => Monk.Instance.Mainnet,
                "OWO" => OneWorldCoin.Instance.Mainnet,
                "PNY" => Peony.Instance.Mainnet,
                "SAPP" => Sapphire.Instance.Mainnet,
                "SUV" => Suvereno.Instance.Mainnet,
                "UCR" => UltraClear.Instance.Mainnet,
                // Test
                "TKYAN" => Kyanite.Instance.Testnet,
                _ => throw new ArgumentOutOfRangeException(nameof(coinType), coinType, "Unknown coin type"),
            };
        }


    }
}
