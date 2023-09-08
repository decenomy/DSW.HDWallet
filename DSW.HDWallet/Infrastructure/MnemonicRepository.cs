using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class MnemonicRepository: IMnemonicRepository
    {
        public Mnemonic GenerateMnemonic(WordCount wordCount)
        {
            return new Mnemonic(Wordlist.English, wordCount);
        }

        public Mnemonic GetMnemonic(string mnemonic)
        {
            return new Mnemonic(mnemonic, Wordlist.English);
        }
    }
}
