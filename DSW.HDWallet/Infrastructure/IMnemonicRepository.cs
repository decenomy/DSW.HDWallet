using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public interface IMnemonicRepository
    {
        Mnemonic GenerateMnemonic(WordCount wordCount);
        Mnemonic GetMnemonic(string mnemonic);
    }
}
