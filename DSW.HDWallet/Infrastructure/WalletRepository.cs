using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
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

        private static BitcoinAddress GetAddress(CoinType coinType, Mnemonic mnemo, string? password = null)
        {
            Network network = CoinNetwork.GetMainnet(coinType);
            ExtKey masterKey = string.IsNullOrEmpty(password) ? mnemo.DeriveExtKey() : mnemo.DeriveExtKey(password);
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network);

            return address;
        }

        public DeriveKeyDetails CreateDeriveKey(CoinType coinType, string masterKey, int index)
        {
            Network network = CoinNetwork.GetMainnet(coinType);
            ExtKey master = ExtKey.Parse(masterKey, network);

            string coin_type = Bip44.GetCoinCodeBySymbol(coinType.ToString());
            KeyPath keyPath = new KeyPath($"m/44'/{coin_type}'/0'/0'/{index}'");

            ExtKey derivedKey = master.Derive(keyPath);
            Key privateKey = derivedKey.PrivateKey;
            PubKey publicKey = privateKey.PubKey;

            BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, network);


            DeriveKeyDetails deriveKeyDetails = new()
            {
                Address = address.ToString(),
                Path = "m/" + keyPath.ToString(),
            };

            return deriveKeyDetails;
        }
    }
}
