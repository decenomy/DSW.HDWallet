using DSW.HDWallet.Domain.Coins;
using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic);
        Wallet CreateWithPassword(Mnemonic mnemonic, string? password = null);
        string Recover(Mnemonic mnemo, string? password = null);
        DeriveKeyDetails CreateDeriveKey(CoinType coinType, Mnemonic mnemo, int index, string? password = null);
        DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index);
        PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex, string? password = null);

    }
}
