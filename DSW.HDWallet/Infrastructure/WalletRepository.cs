using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ICoinRepository coinRepository;

        public WalletRepository(ICoinRepository coinRepository)
        {
            this.coinRepository = coinRepository;
        }

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
       
        

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, string ticker, int Index, bool isNetworkTest = false)
        {
            var changeType = 0;

            Network network = coinRepository.GetNetwork(ticker);
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

        public PubKeyDetails GeneratePubkey(string ticker, string seedHex, string? password = null, bool isNetworkTest = false)
        {
            var purpose = 44;
            var accountIndex = 0;

            Network network = coinRepository.GetNetwork(ticker);
            int coinCode = coinRepository.GetCoinInfo(ticker).Code;

            ExtKey masterPrivKey = new ExtKey(seedHex);

            KeyPath keyPath = new($"m/{purpose}'/{coinCode}'/{accountIndex}'");
            ExtPubKey pubKey = masterPrivKey.Derive(keyPath).Neuter();

            PubKeyDetails pubKeyDetails = new()
            {
                PubKey = pubKey.ToString(network),
                Ticker = ticker,
                Index = 0,
                Path = keyPath.ToString()
            };

            return pubKeyDetails;
        }

        public DeriveKeyDetails CreateDeriveKey(string ticker, Mnemonic mnemo, int index, string? password = null, bool isNetworkTest = false)
        {
            var purpose = 44;
            var accountIndex = 0;
            var changeType = 0;

            Network network = coinRepository.GetNetwork(ticker);
            int coinCode = coinRepository.GetCoinInfo(ticker).Code;

            ExtKey masterPrivKey = string.IsNullOrEmpty(password) ? 
                                   mnemo.DeriveExtKey() : 
                                   mnemo.DeriveExtKey(password);   
            
            KeyPath keyPath = new($"m/{purpose}'/{coinCode}'/{accountIndex}'/{changeType}'/{index}'");

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

        private List<UtxoObject> SelectUtxos(List<UtxoObject> utxos, long targetValue, long fee = 0)
        {
            long totalValue = 0;
            var selectedUtxos = new List<UtxoObject>();

            if(utxos != null && utxos!.Count > 0)
            {
                if (fee > 0)
                    targetValue += fee;

                var exactMatch = utxos.FirstOrDefault(utxo => utxo.Value?.ToLong() == targetValue);

                if (exactMatch != null)
                    return new List<UtxoObject> { exactMatch };

                var sortedUtxos = utxos.OrderByDescending(utxo => utxo.Value!.ToULong()).ToList();


                while (totalValue < targetValue && sortedUtxos.Count > 0)
                {
                    long value = targetValue - totalValue;

                    var closestLowerValueUtxo = sortedUtxos.FirstOrDefault(utxo => utxo.Value!.ToLong() < value);

                    if (closestLowerValueUtxo != null)
                    {
                        selectedUtxos.Add(closestLowerValueUtxo);
                        totalValue += closestLowerValueUtxo.Value!.ToLong();
                        sortedUtxos.Remove(closestLowerValueUtxo);
                    }
                    else
                    {
                        var remainingUtxos = sortedUtxos.OrderBy(utxo => utxo.Value!.ToLong()).ToList();
                        foreach (var utxo in remainingUtxos)
                        {
                            selectedUtxos.Add(utxo);
                            totalValue += utxo.Value!.ToLong();
                            remainingUtxos.Remove(utxo);

                            if (totalValue >= targetValue)
                                break;
                        }
                    }
                }

                if (totalValue < targetValue && sortedUtxos.Count == 0)
                    return new List<UtxoObject>();
            }

            return selectedUtxos;
        }

        public TransactionDetails GenerateTransaction(string ticker, List<UtxoObject> utxos, long amountToSend, string seedHex, string toAddress, long fee = 0)
        {
            Network network = coinRepository.GetNetwork(ticker);
            var utxoSelected = SelectUtxos(utxos, amountToSend, fee);
            
            try
            {
                if (utxoSelected != null && utxoSelected!.Count > 0)
                {
                    ExtKey extendedKey = new(seedHex);
                    var transaction = Transaction.Create(network);
                    var inputs = new List<TxIn>();
                    var key = extendedKey.PrivateKey.GetBitcoinSecret(network);

                    BitcoinAddress changeAddress = extendedKey.PrivateKey.GetAddress(ScriptPubKeyType.Legacy, network);
                    BitcoinAddress recipientAddress = BitcoinAddress.Create(toAddress, network);

                    Money amount = Money.Coins(amountToSend.ToDecimalPoint());
                                        
                    foreach (var utxo in utxoSelected)
                    {
                        OutPoint outPoint = new OutPoint(uint256.Parse(utxo.Txid), utxo.Vout);
                        var txIn = new TxIn(outPoint);
                        transaction.Inputs.Add(txIn);
                    }

                    TxOut txOutRecipient = new TxOut(amount, recipientAddress);
                    transaction.Outputs.Add(txOutRecipient);

                    Money totalInputAmount = utxoSelected.Select(x => x.Value!.ToLong()).Aggregate((total, current) => total + current);
                    Money changeAmount = totalInputAmount - amount;

                    int transactionSizeBytes = transaction.GetSerializedSize();
                    decimal transactionSizeKB = (decimal)transactionSizeBytes / 1000;

                    long transactionFeeSatoshis = (long)(transactionSizeKB * 10000);

                    if (transactionFeeSatoshis > Money.Zero.Satoshi)
                    {
                        changeAmount -= new Money(transactionFeeSatoshis);

                        if (changeAmount < Money.Zero.Satoshi)
                            return GenerateTransaction(ticker, utxos, amountToSend, seedHex, toAddress, changeAmount.Abs());
                    }

                    if (changeAmount > Money.Zero)
                    {
                        TxOut txOutChange = new TxOut(changeAmount, changeAddress);
                        transaction.Outputs.Add(txOutChange);
                    }

                    Coin[] inputCoins = utxoSelected.Select(x => new Coin(new OutPoint(new uint256(x.Txid), x.Vout), txOutRecipient)).ToArray();

                    transaction.Sign(key, inputCoins);

                    TransactionDetails transactionDetails = new()
                    {
                        Transaction = transaction,
                        ToAddress = toAddress,
                        Balance = totalInputAmount,
                        Amount = amount,
                        Change = changeAmount,
                        Fee = new Money(transactionFeeSatoshis),
                        SizeKb = transactionSizeKB,
                        Utxos = utxoSelected,
                        Message = "The transaction was created successfully.",
                        Status = "OK"

                    };

                    return transactionDetails;
                }
                else
                {
                    TransactionDetails transactionDetails = new()
                    {
                        Status = "Error",
                        Message = "The UTXOs or balance are insufficient for this transaction."
                    };
                    
                    return transactionDetails;
                }                
            }
            catch (Exception ex)
            {
                TransactionDetails transactionDetails = new()
                {
                    Status = "Error",
                    Message = ex.Message
                };

                return transactionDetails;
            }
        }

        public bool ValidateAddress(string ticker, string address)
        {
            Network network = coinRepository.GetNetwork(ticker);

            try
            {
                BitcoinAddress bitcoinAddress = BitcoinAddress.Create(address, network);

                if (bitcoinAddress != null)
                    return true;
            }
            catch (FormatException)
            {
                return false;              
            }

            return false;
        }

        public string GetCoinName(string ticker)
        {
            return coinRepository.GetCoinInfo(ticker).Name;
        }

        public List<ICoinExtension> GetAllCoin()
        {
            return coinRepository.GetListCoin();
        }
    }
}
