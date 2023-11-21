using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface ICoinRepository
    {
        Network GetNetwork(string ticker);
        IEnumerable<ICoinExtension> Coins { get; }
        ICoinExtension GetCoin(string ticker);
    }
}
