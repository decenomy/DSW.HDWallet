using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {
        public Wallet CreateWallet()
        {
            throw new NotImplementedException();
        }

        public BitcoinAddress RecoverWalletAddress(string mnemonicWords)
        {
            throw new NotImplementedException();
        }
    }
}
