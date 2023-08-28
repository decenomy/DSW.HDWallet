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

        public Wallet CreateWithPassword(Mnemonic mnemo, string? password = null)
        {            
            BitcoinAddress address = GetAddress(mnemo, password);

            var wallet = new Wallet
            {
                MasterKey = mnemo.DeriveExtKey().ToString(Network.Main),
                Address = address.ToString(),
                SecretWords = mnemo.ToString(),
                SecrectWordsArray = mnemo.Words
            };

            return wallet;
        }

        public BitcoinAddress Recover(Mnemonic mnemo, string? password = null)
        {
            return GetAddress(mnemo, password);
        }

        private static BitcoinAddress GetAddress(Mnemonic mnemo, string? password = null)
        {
            ExtKey masterKey = string.IsNullOrEmpty(password) ? mnemo.DeriveExtKey() : mnemo.DeriveExtKey(password);
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

            return address;
        }

        public BitcoinExtKey CreateDeriveKey(ExtKey masterKey, KeyPath keyPath)
        {
            return new BitcoinExtKey(masterKey.Derive(keyPath), Network.Main);
        }
    }
}
