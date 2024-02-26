using DSW.HDWallet.Application.Extension;
using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;
using NBitcoin.RPC;
using System.Linq;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {
        private readonly IBlockbookHttpClient blockbookHttpClient;
        private readonly ICoinRepository coinRepository;
        private readonly IAddressManager addressManager;

        public WalletService(IBlockbookHttpClient blockbookHttpClient,
            ICoinRepository coinRepository,
            IAddressManager addressManager)
        {
            this.blockbookHttpClient = blockbookHttpClient;
            this.coinRepository = coinRepository;
            this.addressManager = addressManager;
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

        public async Task<TransactionDetails> GenerateTransaction(string ticker, string seedHex, long amountToSend, string toAddress)
        {
            string pubKey = GeneratePubkey(ticker, seedHex).PubKey;
            List<UtxoObject> utxos = (await GetUtxo(ticker, pubKey)).ToList();
            Network network = coinRepository.GetNetwork(ticker);
            var utxoSelected = SelectUtxos(utxos, amountToSend);

            try
            {
                if (utxoSelected == null || !utxoSelected.Any())
                {
                    return new TransactionDetails
                    {
                        Status = "Error",
                        Message = "The UTXOs or balance are insufficient for this transaction."
                    };
                }

                var transaction = Transaction.Create(network);
                Money totalInputAmount = 0L;

                foreach (var utxo in utxoSelected)
                {
                    transaction.Inputs.Add(new TxIn(new OutPoint(uint256.Parse(utxo.Txid), utxo.Vout)));
                    totalInputAmount += new Money(long.Parse(utxo.Value!));
                }

                // Outputs
                BitcoinAddress recipientAddress;
                try
                {
                    recipientAddress = BitcoinAddress.Create(toAddress, network);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Invalid Address");
                }

                Money amount = Money.Coins(amountToSend.ToDecimalPoint());
                transaction.Outputs.Add(new TxOut(amount, recipientAddress));

                AddressInfo? changeAddress = await addressManager.GetUnusedAddress(ticker);

                // Ensure the changeAddress is not in the utxoSelected list
                bool isAddressInUtxoSelected;
                int nextCoinIndex = await addressManager.GetCoinIndex(ticker);

                do
                {
                    nextCoinIndex++;

                    changeAddress = await addressManager.GetAddress(pubKey, ticker, nextCoinIndex, true);

                    // Check if the newly generated change address is in the utxoSelected list
                    isAddressInUtxoSelected = utxoSelected.Any(utxo => utxo.Address == changeAddress.Address);

                } while (isAddressInUtxoSelected);

                Money changeAmount = totalInputAmount - amount;
                if (changeAmount > Money.Zero)
                {
                    transaction.Outputs.Add(new TxOut(changeAmount, BitcoinAddress.Create(changeAddress.Address, network)));
                }

                // Fee calculation
                FeeResultObject feeResult = await blockbookHttpClient.GetFeeEstimation(ticker, 1);
                long fee = 0L;
                int transactionSizeBytes = 0;

                // If the fee estimation returns -1, use the minimum network fee
                if (feeResult.Result == "-1") {
                    const long minFeePerByte = 10;
                    transactionSizeBytes = transaction.GetSerializedSize();
                    fee = transactionSizeBytes * minFeePerByte;
                }
                else
                {
                    // Use the estimated fee from the response
                    long feePerByte = long.Parse(feeResult.Result!);
                    transactionSizeBytes = transaction.GetSerializedSize();
                    fee = transactionSizeBytes * feePerByte;
                }

                // Update the change output after fee calculation
                if (transaction.Outputs.Count > 1 && changeAmount > new Money(fee))
                {
                    transaction.Outputs[1].Value -= new Money(fee);
                }
                else if (transaction.Outputs.Count == 1 && totalInputAmount > amount + new Money(fee))
                {
                    // If no change output, reduce the amount sent to include the fee
                    transaction.Outputs[0].Value = amount - new Money(fee);
                }

                // Signing the transaction
                foreach (var group in utxoSelected.GroupBy(u => u.Path))
                {
                    BitcoinSecret key = WalletUtils.DerivePrivateKey(seedHex, group.Key!, network);
                    transaction.Sign(key, group.Select(u => new Coin(new uint256(u.Txid), (uint)u.Vout, new Money(long.Parse(u.Value!)), PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(key.PubKey.Hash))).ToArray());
                }

                return new TransactionDetails
                {
                    Transaction = transaction,
                    ToAddress = toAddress,
                    ChangeAddress = changeAddress,
                    Balance = totalInputAmount,
                    Amount = amount,
                    Change = transaction.Outputs.Count > 1 ? transaction.Outputs[1].Value : Money.Zero,
                    Fee = new Money(fee),
                    SizeKb = (decimal)(transactionSizeBytes / 1000.0),
                    Utxos = utxoSelected,
                    Message = "The transaction was created successfully.",
                    Status = "OK"
                };
            }
            catch (Exception ex)
            {
                return new TransactionDetails
                {
                    Status = "Error",
                    Message = ex.Message
                };
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