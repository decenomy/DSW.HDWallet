using DSW.HDWallet.Infrastructure.Coins;

namespace DSW.HDWallet.Domain.Coins;

public interface ICoinExtension
{
    public string Ticker { get; }
    public int Code { get; }
    public string HexCode { get; }
    public string Name { get; }
    public string Image { get; }
    public string CoinGeckoId { get; }
    public bool IsTestNet { get; }
}

