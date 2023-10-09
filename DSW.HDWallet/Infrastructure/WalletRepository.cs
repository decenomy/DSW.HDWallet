using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.ApiObjects;
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
       
        public PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex, string? password = null, bool isNetworkTest = false)
        {
            var purpose = 44;
            var accountIndex = 0;

            Network network = CoinNetwork.GetMainnet(coinType, isNetworkTest);
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

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index, bool isNetworkTest = false)
        {
            var changeType = 0;

            Network network = CoinNetwork.GetMainnet(coinType, isNetworkTest);
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

        
        public DeriveKeyDetails CreateDeriveKey(CoinType coinType, Mnemonic mnemo, int index, string? password = null, bool isNetworkTest = false)
        {
            var purpose = 44;
            var accountIndex = 0;
            var changeType = 0;

            Network network = CoinNetwork.GetMainnet(coinType, isNetworkTest);
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

        public List<UtxoObject> Transaction(ulong value, List<UtxoObject> utxos)
        {
            return SelectUtxos(utxos, value);
        }

        private List<UtxoObject> SelectUtxos(List<UtxoObject> utxos, ulong targetValue)
        {
            ulong totalValue = 0;
            var selectedUtxos = new List<UtxoObject>();

            var exactMatch = utxos.FirstOrDefault(utxo => utxo.Value!.ToULong() == targetValue);

            if (exactMatch != null)
                return new List<UtxoObject> { exactMatch };

            var sortedUtxos = utxos.OrderByDescending(utxo => utxo.Value!.ToULong()).ToList();


            while (totalValue < targetValue)
            {
                ulong value = targetValue - totalValue;

                var closestLowerValueUtxo = sortedUtxos.FirstOrDefault(utxo => utxo.Value!.ToULong() < value);

                if (closestLowerValueUtxo != null)
                {
                    selectedUtxos.Add(closestLowerValueUtxo);
                    totalValue += closestLowerValueUtxo.Value!.ToULong();
                    sortedUtxos.Remove(closestLowerValueUtxo);
                }
                else
                {
                    var remainingUtxos = sortedUtxos.OrderBy(utxo => utxo.Value!.ToULong()).ToList();
                    foreach (var utxo in remainingUtxos)
                    {
                        selectedUtxos.Add(utxo);
                        totalValue += utxo.Value!.ToULong();
                        remainingUtxos.Remove(utxo); 

                        if (totalValue >= targetValue)
                            break;
                    }
                }
            }
            return selectedUtxos;
        }



    }
}
