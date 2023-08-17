using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface IAddressRepository
    {
        BitcoinAddress GetAddressFromMnemonic(Mnemonic mnemonic);
    }
}
