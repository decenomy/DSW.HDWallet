using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic);
        BitcoinAddress Recover(Mnemonic mnemonic);
    }
}
