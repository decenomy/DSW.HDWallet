using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface IMnemonicRepository
    {
        Mnemonic GenerateMnemonic();
        Mnemonic GetMnemonic(string secretWords);
    }
}
