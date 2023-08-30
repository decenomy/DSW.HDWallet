using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
        public Wallet Create(Mnemonic mnemo, CoinType coinType)
        {
            Network network = CoinNetwork.GetMainnet(coinType);
            BitcoinAddress address = GetAddress(coinType, mnemo);

            var wallet = new Wallet
            {
                MasterKey = mnemo.DeriveExtKey().ToString(network),
                Address = address.ToNetwork(network).ToString(),
                SecretWords = mnemo.ToString(),
                SecrectWordsArray = mnemo.Words
            };

            return wallet;
        }

        public Wallet CreateWithPassword(CoinType coinType, Mnemonic mnemo, string? password = null)
        {
            Network network = CoinNetwork.GetMainnet(coinType);
            BitcoinAddress address = GetAddress(coinType, mnemo, password);

            var wallet = new Wallet
            {
                MasterKey = mnemo.DeriveExtKey().ToString(network),
                Address = address.ToString(),
                SecretWords = mnemo.ToString(),
                SecrectWordsArray = mnemo.Words
            };

            return wallet;
        }

        public BitcoinAddress Recover(CoinType coinType, Mnemonic mnemo, string? password = null)
        {
            return GetAddress(coinType, mnemo, password);
        }

        private static BitcoinAddress GetAddress(CoinType coinType, Mnemonic mnemo, string? password = null)
        {
            Network network = CoinNetwork.GetMainnet(coinType);
            ExtKey masterKey = string.IsNullOrEmpty(password) ? mnemo.DeriveExtKey() : mnemo.DeriveExtKey(password);
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, network);

            return address;
        }

        public BitcoinExtKey CreateDeriveKey(CoinType coinType, ExtKey masterKey, KeyPath keyPath)
        {
            Network network = CoinNetwork.GetMainnet(coinType);

            return new BitcoinExtKey(masterKey.Derive(keyPath), network);
        }
    }
}
