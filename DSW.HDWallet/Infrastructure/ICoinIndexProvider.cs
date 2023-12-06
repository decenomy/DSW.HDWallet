using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface ICoinIndexProvider
    {
        int GetCoinIndex(string ticker);
    }
}
