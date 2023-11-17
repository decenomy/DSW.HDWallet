using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface ICoinRepository
    {
        public Network GetNetwork(string ticker);

        public List<ICoinExtension> GetListCoin();
    }
}
