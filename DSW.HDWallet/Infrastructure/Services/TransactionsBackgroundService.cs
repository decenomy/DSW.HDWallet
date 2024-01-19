using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class TransactionsBackgroundService : BaseBackgroundService<TransactionsBackgroundService>
    {
        private readonly IBlockbookHttpClient blockbookHttpClient;
        private readonly IStorage storage;

        public TransactionsBackgroundService(
            IBlockbookHttpClient blockbookHttpClient,
            IStorage storage,
            ILogger<TransactionsBackgroundService> logger
        ) : base(logger, "0 */1 * * * *") //Cron expression to make the service run every 5 minutes
        {
            this.blockbookHttpClient = blockbookHttpClient;
            this.storage = storage;
        }

        protected override async Task OnExecute(CancellationToken cancellationToken)
        {
            logger.LogTrace("Rates Update Service executing.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var wallets = await storage.GetAllWallets();
                    foreach (var wallet in wallets)
                    {
                        var coinAddresses = await storage.GetAddressesByTicker(wallet.Ticker!);
                        var addresses = coinAddresses.Select(ca => ca.Address).ToList();
                        var xpub = await blockbookHttpClient.GetXpub(wallet.Ticker!, wallet.PublicKey!);

                        foreach (var txid in xpub.Txids!)
                        {
                            var existingTransaction = await storage.GetTransactionByTxId(txid);
                            if (existingTransaction == null)
                            {
                                var transactionDetails = await blockbookHttpClient.GetTransactionAsync(wallet.Ticker!, txid);

                                var transactionType = DetermineTransactionType(transactionDetails, addresses);
                                var transactionAmount = CalculateTransactionAmount(transactionDetails, addresses, transactionType);

                                var transactionRecord = new TransactionRecord
                                {
                                    TxId = transactionDetails.Txid,
                                    Ticker = wallet.Ticker,
                                    Type = transactionType,// "teste", //TODO Swap for the enumerator
                                    Amount = transactionAmount,
                                    FromAddress = transactionDetails.Vin != null && transactionDetails.Vin.Any() ? transactionDetails.Vin.First().Addresses?.FirstOrDefault() ?? string.Empty : string.Empty,
                                    ToAddress = transactionDetails.Vout != null && transactionDetails.Vout.Any() ? transactionDetails.Vout.First().Addresses?.FirstOrDefault() ?? string.Empty : string.Empty,
                                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(transactionDetails.BlockTime).UtcDateTime,
                                    IsConfirmed = transactionDetails.Confirmations > 0,
                                    TransactionFee = Convert.ToDecimal(transactionDetails.Fees) / 100000000, //TODO Example conversion
                                    Notes = "" // Any additional information or leave empty
                                };

                                await storage.AddTransaction(transactionRecord);
                            }
                        }
                    }

                    var t = DateTime.Now;
                    await Task.Delay(schedule.GetNextOccurrence(t) - t, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            logger.LogTrace("Rates Update Service executed.");
        }

        private TransactionType DetermineTransactionType(TransactionObject transactionDetails, List<string> walletAddresses)
        {
            // Mining transaction: no Vins and at least one Vout with an address from my wallet
            bool isMining = !transactionDetails.Vin.Any()  && transactionDetails.Vout.Any(vout => vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

            // Staking transaction: Has one Vin, the first Vout has a value of 0 and an empty script, 
            // One or more Vouts with the same address as Vin and the sum is greater than the value of Vin
            bool isStaking = transactionDetails.Vin.Count == 1
                && transactionDetails.Vout.Count > 0
                && transactionDetails.Vout.First().Value == "0"
                && transactionDetails.Vout.Skip(1).Any(vout => vout.Addresses.Contains(transactionDetails.Vin.First().Addresses.First()))
                && transactionDetails.Vin.Sum(v => Convert.ToDecimal(v.Value)) < transactionDetails.Vout.Sum(v => Convert.ToDecimal(v.Value));

            // Masternode Reward: It is a staking transaction but has a Vout with an address from my wallet (masternode reward)
            bool isMasternodeReward = isStaking && transactionDetails.Vout.Any(vout => vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

            // Internal transaction: All addresses from Vins and Vouts belong to my wallet
            bool isInternal = transactionDetails.Vin.All(vin => vin.Addresses.Any(addr => walletAddresses.Contains(addr)))
                && transactionDetails.Vout.All(vout => vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

            // Outgoing transaction: All Vins are from my wallet and there is at least one address in VOUTs that is not from my wallet
            bool isOutgoing = transactionDetails.Vin.All(vin => vin.Addresses.Any(addr => walletAddresses.Contains(addr)))
                && transactionDetails.Vout.Any(vout => vout.Addresses.All(addr => !walletAddresses.Contains(addr)));

            // Incoming transaction: At least one address from Vouts belongs to the wallet and there are no Vins from the wallet
            bool isIncoming = transactionDetails.Vout.Any(vout => vout.Addresses.Any(addr => walletAddresses.Contains(addr)))
                && transactionDetails.Vin.All(vin => vin.Addresses.All(addr => !walletAddresses.Contains(addr)));

            if (isMining) return TransactionType.Mining;
            if (isMasternodeReward) return TransactionType.MasternodeReward;
            if (isStaking) return TransactionType.Staking;
            if (isInternal) return TransactionType.Internal;
            if (isOutgoing) return TransactionType.Outgoing;
            if (isIncoming) return TransactionType.Incoming;

            return TransactionType.Unknown;
        }

        private decimal CalculateTransactionAmount(TransactionObject transactionDetails, List<string> walletAddresses, TransactionType transactionType)
        {
            decimal amount = 0;

            switch (transactionType)
            {
                case TransactionType.Incoming:
                    if (transactionDetails.Vout != null)
                    {
                        amount = transactionDetails.Vout
                                    .Where(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr)))
                                    .Sum(vout => Convert.ToDecimal(vout.Value));
                    }
                    break;

                case TransactionType.Outgoing:
                    if (transactionDetails.Vin != null)
                    {
                        amount = transactionDetails.Vin
                                    .Where(vin => vin.Addresses != null && vin.Addresses.Any(addr => walletAddresses.Contains(addr)))
                                    .Sum(vin => Convert.ToDecimal(vin.Value));
                    }

                    amount -= transactionDetails.Fees != null ? Convert.ToDecimal(transactionDetails.Fees) : 0;
                    break;

                case TransactionType.Internal:
                    amount = 0;
                    break;

                case TransactionType.Mining:
                case TransactionType.Staking:
                case TransactionType.MasternodeReward:
                    if (transactionDetails.Vout != null)
                    {
                        amount = transactionDetails.Vout.Sum(vout => Convert.ToDecimal(vout.Value));
                    }
                    break;

                default:
                    break;
            }

            return amount; // SatoshiConverter.ToSatoshi(amount);
        }



    }
}