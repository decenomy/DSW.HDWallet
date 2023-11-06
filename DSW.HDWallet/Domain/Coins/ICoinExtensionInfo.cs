namespace DSW.HDWallet.Domain.Coins;

public interface ICoinExtensionInfo
{
    public int Code { get; set; }
    public string? HexCode { get; set; }
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? CoinGeckoId { get; set; }
}
