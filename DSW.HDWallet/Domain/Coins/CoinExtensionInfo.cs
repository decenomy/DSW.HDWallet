using DSW.HDWallet.Infrastructure.Coins;

namespace DSW.HDWallet.Domain.Coins;

public class CoinExtensionInfo: ICoinExtensionInfo
{
    public int Code { get; set; }
    public string? HexCode { get; set; }
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? CoinGeckoId { get; set; }

    public static string GetNameByTicker(string ticker)
    {
        return GetCoinInfo(ticker).Name!;
    }

    public static string GetSymbolByTicker(string ticker)
    {
        return GetCoinInfo(ticker).Symbol!;
    }

    public static CoinExtensionInfo GetCoinInfo(string symbol) => symbol switch
    {
        "AZR" => Azzure.GetCoinInfo(),
        "BECN" => Beacon.GetCoinInfo(),
        "BIR" => Birake.GetCoinInfo(),
        "CFL" => CryptoFlow.GetCoinInfo(),
        "SAGA" => CryptoSaga.GetCoinInfo(),
        "DASHD" => DashDiamond.GetCoinInfo(),
        "ESK" => EskaCoin.GetCoinInfo(),
        "FLS" => Flits.GetCoinInfo(),
        "777" => Jackpot.GetCoinInfo(),
        "KYAN" => Kyanite.GetCoinInfo(),
        "MOBIC" => MobilityCoin.GetCoinInfo(),
        "MONK" => Monk.GetCoinInfo(),
        "OWO" => OneWorldCoin.GetCoinInfo(),
        "PNY" => Peony.GetCoinInfo(),
        "SAPP" => Sapphire.GetCoinInfo(),
        "SUV" => Suvereno.GetCoinInfo(),
        "UCR" => UltraClear.GetCoinInfo(),
        _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, "Unknown coin symbol"),
    };
}

