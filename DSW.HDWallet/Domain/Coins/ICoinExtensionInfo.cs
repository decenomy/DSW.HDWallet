using NBitcoin;

namespace DSW.HDWallet.Domain.Coins;

public class ICoinExtensionInfo : NetworkSetBase
{
    public override string? CryptoCode { get; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public string? CoinGeckoId { get; set; }
    protected override NetworkBuilder CreateMainnet()
    {
        throw new NotImplementedException();
    }

    protected override NetworkBuilder CreateRegtest()
    {
        throw new NotImplementedException();
    }

    protected override NetworkBuilder CreateTestnet()
    {
        throw new NotImplementedException();
    }
}
