using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Domain.WSObject;
using DSW.HDWallet.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.WS;
using NBitcoin;
using System.Security.Cryptography.X509Certificates;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {
        private readonly IBlockbookHttpClient blockbookHttpClient;
        private readonly ICoinRepository coinRepository;
        private readonly ICoinAddressManager coinAddressManager;

        public WalletService(IBlockbookHttpClient blockbookHttpClient,
            ICoinRepository coinRepository,
            ICoinAddressManager coinAddressManager)
        {
            this.blockbookHttpClient = blockbookHttpClient;
            this.coinRepository = coinRepository;
            this.coinAddressManager = coinAddressManager;
        }

        public Wallet CreateWallet(WordCount wordCount, string? password = null)
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, wordCount);

            return new Wallet
            {
                SeedHex = mnemo.DeriveSeed(password).ToHexString(),
                Mnemonic = mnemo.ToString(),
                MnemonicArray = mnemo.Words
            };
        }

        public string GetSeedHex(Mnemonic mnemo, string? password = null) => mnemo.DeriveSeed(password).ToHexString();

        public string RecoverWallet(string mnemonic, string? password = null)
        {
            Mnemonic mnemo = new Mnemonic(mnemonic, Wordlist.English);

            return GetSeedHex(mnemo, password);
        }

        public PubKeyDetails GeneratePubkey(string ticker, string seedHex)
        {
            var purpose = 44;
            var accountIndex = 0;

            Network network = coinRepository.GetNetwork(ticker);
            int coinCode = coinRepository.GetCoin(ticker).Code;

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

        public AddressInfo GetAddress(string pubKey, string ticker, int index, bool isChange = false)
        {
            var changeType = isChange ? 1 : 0;

            Network network = coinRepository.GetNetwork(ticker);
            ExtPubKey extPubKey = ExtPubKey.Parse(pubKey, network);

            var keypath = $"{changeType}/{index}";

            var address = extPubKey.Derive(new KeyPath(keypath))
                                    .GetPublicKey()
                                    .GetAddress(ScriptPubKeyType.Legacy, network);

            AddressInfo deriveKeyDetails = new()
            {
                Address = address.ToString(),
                Index = index
            };

            return deriveKeyDetails;
        }

        public async Task<AddressObject> GetAddressAsync(string coin, string address)
        {
            return await blockbookHttpClient.GetAddressAsync(coin, address);
        }

        public async Task<TransactionObject> GetTransactionAsync(string coin, string txid)
        {
            return await blockbookHttpClient.GetTransactionAsync(coin, txid);
        }

        public async Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid)
        {
            return await blockbookHttpClient.GetTransactionSpecificAsync(coin, txid);
        }

        public async Task<BlockHashObject> GetBlockHash(string coin, string blockHeight)
        {
            return await blockbookHttpClient.GetBlockHash(coin, blockHeight);
        }

        public async Task<XpubObject> GetXpub(string coin, string xpub, int page = 1, int pageSize = 1000)
        {
            return await blockbookHttpClient.GetXpub(coin, xpub, page, pageSize);
        }

        public async Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed = false)
        {
            return await blockbookHttpClient.GetUtxo(coin, address, confirmed);
        }

        //public async Task<WSTransactionObject> GetWSTransactionAsync(string coin, string txId)
        //{
        //    return await _webSocket.GetWSTransactionAsync(coin, txId);
        //}

        //public async Task<WSSubscribeObject> SubscribeNewTransaction(string coin)
        //{
        //    return await _webSocket.SubscribeNewTransaction(coin);
        //}

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

        public async Task<TransactionDetails> GenerateTransaction(string ticker, string seedHex, long amountToSend, string toAddress, long fee = 0)
        {
            string pubKey = GeneratePubkey(ticker, seedHex).PubKey;
            List<UtxoObject> utxos = (await GetUtxo(ticker, pubKey)).ToList();
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

                    AddressInfo? changeAddress = await coinAddressManager.GetUnusedAddress(ticker);

                    if(changeAddress == null)
                    {
                        changeAddress = GetAddress(pubKey, ticker, await coinAddressManager.GetCoinIndex(ticker), true);
                    }
                    
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
                            return await GenerateTransaction(ticker, seedHex, amountToSend, toAddress, changeAmount.Abs());
                    }

                    if (changeAmount > Money.Zero)
                    {
                        TxOut txOutChange = new TxOut(changeAmount, BitcoinAddress.Create(changeAddress.Address, network));
                        transaction.Outputs.Add(txOutChange);
                    }

                    Coin[] inputCoins = utxoSelected.Select(x => new Coin(new OutPoint(new uint256(x.Txid), x.Vout), txOutRecipient)).ToArray();

                    transaction.Sign(key, inputCoins);

                    TransactionDetails transactionDetails = new()
                    {
                        Transaction = transaction,
                        ToAddress = toAddress,
                        ChangeAddress = changeAddress,
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

        private List<UtxoObject> SelectUtxos(List<UtxoObject> utxos, long targetValue, long fee = 0)
        {
            long totalValue = 0;
            var selectedUtxos = new List<UtxoObject>();

            if (utxos != null && utxos!.Count > 0)
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

    }
}