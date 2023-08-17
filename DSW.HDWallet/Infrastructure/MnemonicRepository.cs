using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class MnemonicRepository: IAddressRepository
    {
        public Mnemonic GenerateMnemonic()
        {
            return new Mnemonic(Wordlist.English, WordCount.Twelve);
        }

        public BitcoinAddress GetAddressFromMnemonic(Mnemonic mnemonic)
        {
            ExtKey masterKey = mnemonic.DeriveExtKey();
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            return address;
        }
    }
}
