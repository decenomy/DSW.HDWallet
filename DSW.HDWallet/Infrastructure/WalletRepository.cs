using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure.Api;
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
       
        public PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex, string? password = null)
        {
            var purpose = 44;
            var accountIndex = 0;

            Network network = CoinNetwork.GetMainnet(coinType);
            string coin_type = Bip44.GetCoinCodeBySymbol(coinType.ToString());

            ExtKey masterPrivKey = new ExtKey(seedHex);

            KeyPath keyPath = new($"m/{purpose}'/{coin_type}'/{accountIndex}'");
            ExtPubKey pubKey = masterPrivKey.Derive(keyPath).Neuter();

            PubKeyDetails pubKeyDetails = new()
            {
                PubKey = pubKey.ToString(network),
                CoinType = coinType,
                Index = 0,
                Path = keyPath.ToString()
            };

            return pubKeyDetails;
        }

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index)
        {
            var changeType = 0;

            Network network = CoinNetwork.GetMainnet(coinType);
            ExtPubKey extPubKey = ExtPubKey.Parse(pubKey, network);

            var keypath = $"{changeType}/{Index}";

            var address =  extPubKey.Derive(new KeyPath(keypath))
                                    .GetPublicKey()
                                    .GetAddress(ScriptPubKeyType.Legacy, network);


            DeriveKeyDetailsApp deriveKeyDetails = new()
            {
                Address = address.ToString(),
                Path = keypath.ToString()
            };

            return deriveKeyDetails;
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
