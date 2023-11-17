using DSW.HDWallet.Infrastructure.Coins;

namespace DSW.HDWallet.Domain.Coins;

public interface ICoinExtension
{
    public string Ticker { get; set; }
    public int Code { get; set; }
    public string HexCode { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string CoinGeckoId { get; set; }
    public bool IsTestNet { get; set; }


}

