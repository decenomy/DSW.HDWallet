using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic);
        Wallet CreateWithPassword(Mnemonic mnemonic, string? password = null);
        BitcoinAddress Recover(Mnemonic mnemo, string? password = null);
        DeriveKeyDetails CreateDeriveKey(CoinType coinType, string masterKey, int index);
    }
}
