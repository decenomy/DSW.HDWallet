using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class CoinRepository : ICoinRepository
    {
        public Network GetNetwork(string ticker)
        {
            throw new NotImplementedException();
        }

        public List<ICoinExtension> GetListCoin()
        {
            throw new NotImplementedException();
        }

        public ICoinExtension GetCoinInfo(string ticker) {
            throw new NotImplementedException();
        }
    }
}
