using DSW.HDWallet.Infrastructure.Coins;

namespace DSW.HDWallet.Domain.Coins;

public interface ICoinExtension
{
    string Ticker { get; }
    int Code { get; }
    string Name { get; }
    string Image { get; }
    string CoinGeckoId { get; }
    bool IsTestNet { get; }
}

