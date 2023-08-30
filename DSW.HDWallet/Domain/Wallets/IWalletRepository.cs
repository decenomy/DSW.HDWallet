using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic, CoinType coinType);
        Wallet CreateWithPassword(CoinType coinType, Mnemonic mnemonic, string? password = null);
        BitcoinAddress Recover(CoinType coinType, Mnemonic mnemo, string? password = null);
        BitcoinExtKey CreateDeriveKey(CoinType coinType, ExtKey masterKey, KeyPath keyPath);
    }
}
