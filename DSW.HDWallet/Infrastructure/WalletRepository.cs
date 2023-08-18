using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
        // TO DO - Configurar a injeção de dependência para fornecer a instância de Network quando você instancia a classe WalletRepository

        /*
        private readonly Network _network;

        public WalletRepository(Network network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
        }
        */

        public Wallet Create(Mnemonic mnemo)
        {
            BitcoinAddress address = GetAddress(mnemo);

            var wallet = new Wallet
            {
                MasterKey = mnemo.DeriveExtKey().ToString(Network.Main),
                Address = address.ToString(),
                SecretWords = mnemo.ToString(),
                SecrectWordsArray = mnemo.Words
            };

            return wallet;
        }

        public BitcoinAddress Recover(Mnemonic mnemo)
        {
            return GetAddress(mnemo);
        }

        private static BitcoinAddress GetAddress(Mnemonic mnemo)
        {
            ExtKey masterKey = mnemo.DeriveExtKey();
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

            return address;
        }
    }
}
