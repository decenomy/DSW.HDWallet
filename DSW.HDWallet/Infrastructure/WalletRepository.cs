using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
        public Wallet Create(Mnemonic mnemo)
        {
            byte[] seed = mnemo.DeriveSeed();
            string seedHex = seed.ToHexString();

            var wallet = new Wallet
            {
                SeedHex = seedHex,
                Mnemonic = mnemo.ToString(),
                MnemonicArray = mnemo.Words
            };

            return wallet;
        }

        public Wallet CreateWithPassword(Mnemonic mnemo, string? password = null)
        {
            byte[] seed = mnemo.DeriveSeed(password);
            string seedHex = seed.ToHexString();

            var wallet = new Wallet
            {
                SeedHex = seedHex,
                Mnemonic = mnemo.ToString(),
                MnemonicArray = mnemo.Words
            };

            return wallet;
        }

        public string Recover(Mnemonic mnemo, string? password = null)
        {
            byte[] seed = string.IsNullOrEmpty(password) ? mnemo.DeriveSeed() : mnemo.DeriveSeed(password);

            return seed.ToHexString();
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

        public DeriveKeyDetails CreateDeriveKey(CoinType coinType, Mnemonic mnemo, int index, string? password = null)
        {
            var purpose = 44;
            var accountIndex = 0;
            var changeType = 0;

            Network network = CoinNetwork.GetMainnet(coinType);
            string coin_type = Bip44.GetCoinCodeBySymbol(coinType.ToString());

            ExtKey masterPrivKey = string.IsNullOrEmpty(password) ? 
                                   mnemo.DeriveExtKey() : 
                                   mnemo.DeriveExtKey(password);   
            
            KeyPath keyPath = new($"m/{purpose}'/{coin_type}'/{accountIndex}'/{changeType}'/{index}'");

            var derivedKey = masterPrivKey.Derive(keyPath).Neuter();
            PubKey publicKey = derivedKey.PubKey;
            BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, network);
            var extPubkey = derivedKey.ToString(network);

            DeriveKeyDetails deriveKeyDetails = new()
            {
                PubKey = extPubkey.ToString(),
                Address = address.ToString(),
                Path = "m/" + keyPath.ToString(),
            };

            return deriveKeyDetails;
        }
    }
}
