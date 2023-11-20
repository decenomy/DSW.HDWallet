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

    //public static string GetNameByTicker(string ticker)
    //{
    //    return GetCoinInfo(ticker).Name!;
    //}

    //public static string GetSymbolByTicker(string ticker)
    //{
    //    return GetCoinInfo(ticker).Symbol!;
    //}

    //public static int GetCodeByTicker(string ticker)
    //{
    //    return GetCoinInfo(ticker).Code;
    //}

    //public List<CoinExtensionInfo> GetAllCoin()
    //{
    //    var coinInfoList = new List<CoinExtensionInfo>
    //        {
    //            Azzure.GetCoinInfo(),
    //            Beacon.GetCoinInfo(),
    //            Birake.GetCoinInfo(),
    //            CryptoFlow.GetCoinInfo(),
    //            CryptoSaga.GetCoinInfo(),
    //            DashDiamond.GetCoinInfo(),
    //            EskaCoin.GetCoinInfo(),
    //            Flits.GetCoinInfo(),
    //            Jackpot.GetCoinInfo(),
    //            Kyanite.GetCoinInfo(),
    //            MobilityCoin.GetCoinInfo(),
    //            Monk.GetCoinInfo(),
    //            OneWorldCoin.GetCoinInfo(),
    //            Peony.GetCoinInfo(),
    //            Sapphire.GetCoinInfo(),
    //            Suvereno.GetCoinInfo(),
    //            UltraClear.GetCoinInfo(),
    //            TKyanite.GetCoinInfo(),
    //        };

    //    return coinInfoList;
    //}

    //public static CoinExtensionInfo GetCoinInfo(string symbol) => symbol switch
    //{
    //    "AZR" => Azzure.GetCoinInfo(),
    //    "BECN" => Beacon.GetCoinInfo(),
    //    "BIR" => Birake.GetCoinInfo(),
    //    "CFL" => CryptoFlow.GetCoinInfo(),
    //    "SAGA" => CryptoSaga.GetCoinInfo(),
    //    "DASHD" => DashDiamond.GetCoinInfo(),
    //    "ESK" => EskaCoin.GetCoinInfo(),
    //    "FLS" => Flits.GetCoinInfo(),
    //    "777" => Jackpot.GetCoinInfo(),
    //    "KYAN" => Kyanite.GetCoinInfo(),
    //    "MOBIC" => MobilityCoin.GetCoinInfo(),
    //    "MONK" => Monk.GetCoinInfo(),
    //    "OWO" => OneWorldCoin.GetCoinInfo(),
    //    "PNY" => Peony.GetCoinInfo(),
    //    "SAPP" => Sapphire.GetCoinInfo(),
    //    "SUV" => Suvereno.GetCoinInfo(),
    //    "UCR" => UltraClear.GetCoinInfo(),
    //    // Test Coin
    //    "TKYAN" => TKyanite.GetCoinInfo(),
    //    _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, "Unknown coin symbol"),
    //};
}

