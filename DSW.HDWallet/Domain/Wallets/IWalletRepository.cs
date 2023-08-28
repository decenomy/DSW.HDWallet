using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic);
        Wallet CreateWithPassword(Mnemonic mnemonic, string? password = null);
        BitcoinAddress Recover(Mnemonic mnemo, string? password = null);
        BitcoinExtKey CreateDeriveKey(ExtKey masterKey, KeyPath keyPath);
    }
}
