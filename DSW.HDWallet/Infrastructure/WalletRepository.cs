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

        public List<UtxoObject> SendTransaction(ulong value, List<UtxoObject> utxos)
        {
            var utxoSelected = SelectUtxos(utxos, value);

            CreateTransaction(utxoSelected);

            return utxoSelected;
        }

        private List<UtxoObject> SelectUtxos(List<UtxoObject> utxos, ulong targetValue)
        {
            ulong totalValue = 0;
            var selectedUtxos = new List<UtxoObject>();

            if(utxos != null && utxos!.Count > 0)
            {
                var exactMatch = utxos.FirstOrDefault(utxo => utxo.Value?.ToULong() == targetValue);

                if (exactMatch != null)
                    return new List<UtxoObject> { exactMatch };

                var sortedUtxos = utxos.OrderByDescending(utxo => utxo.Value!.ToULong()).ToList();


                while (totalValue < targetValue && sortedUtxos.Count > 0)
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
            }

            return selectedUtxos;
        }

        public void CreateTransaction(List<UtxoObject> utxos)
        {
            Network network = CoinNetwork.GetMainnet(CoinType.KYAN, true);

            try
            {
                string seedHex = "03da1ed344a3094a4869339844849b98499fc8d56309d6951fabefec35d7f5f3302a8870cb8e64e8e6015295300690feea202ec93af818dc92546ba36143a7fd";
                ExtKey extendedKey = new ExtKey(seedHex);

                // Chave privada do remetente para assinar a transação
                var key = extendedKey.PrivateKey.GetBitcoinSecret(network);

                // Endereço de troco
                BitcoinAddress changeAddress = extendedKey.PrivateKey.GetAddress(ScriptPubKeyType.Legacy, network);

                // Endereço público do destinatário
                BitcoinAddress recipientAddress = BitcoinAddress.Create("Kjs13q3bxt9Hcpsy9EKJ9fvPBBgnoKLiB9", network);

                // Valor a ser enviado em KYAN
                Money amountToSend = Money.Coins(1.0m); // Apenas 1 KYAN

                // Instância da transação
                var transaction = Transaction.Create(network);

                // Entrada de fundos (inputs)
                var inputs = new List<TxIn>();

                foreach (var utxo in utxos)
                {
                    // Entrada de fundos (inputs)
                    OutPoint outPoint = new OutPoint(uint256.Parse(utxo.Txid), utxo.Vout);
                    var txIn = new TxIn(outPoint);
                    transaction.Inputs.Add(txIn);
                }

                // Saída de fundos para o destinatário
                TxOut txOutRecipient = new TxOut(amountToSend, recipientAddress);
                transaction.Outputs.Add(txOutRecipient);

                // Calcular o valor de troco
                Money totalInputAmount = utxos.Select(x => x.Value!.ToULong()).Aggregate((total, current) => total + current);
                Money changeAmount = totalInputAmount - amountToSend;

                // Calcular o tamanho da transação em bytes
                int transactionSizeBytes = transaction.GetSerializedSize();

                // Converter o tamanho da transação para KB
                decimal transactionSizeKB = (decimal)transactionSizeBytes / 1000;

                // Calcular a taxa da transação com base na regra de 10.000 satoshis por KB
                long transactionFeeSatoshis = (long)(transactionSizeKB * 10000);

                if (transactionFeeSatoshis > Money.Zero.Satoshi)
                {
                    // Subtrair a taxa do valor de troco ou da saída para o destinatário

                    //
                    // IMPORTANTE: O Fee deve ser subtraido do valor do troco?
                    //
                    changeAmount -= new Money(transactionFeeSatoshis);
                }

                if (changeAmount > Money.Zero)
                {
                    // Saída de fundos para o endereço de troco
                    TxOut txOutChange = new TxOut(changeAmount, changeAddress);
                    transaction.Outputs.Add(txOutChange);
                }

                // Obtenha as moedas de entrada (as saídas das transações de origem)
                Coin[] inputCoins = utxos.Select(x => new Coin(new OutPoint(new uint256(x.Txid), x.Vout), txOutRecipient)).ToArray();

                // Assine a transação com a moeda de entrada
                transaction.Sign(key, inputCoins);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
    }
}
