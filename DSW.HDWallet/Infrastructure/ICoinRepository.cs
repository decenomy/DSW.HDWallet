using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface ICoinRepository
    {
        Network GetNetwork(string ticker);
        List<ICoinExtension> GetListCoin();
        ICoinExtension GetCoinInfo(string ticker);
    }
}
