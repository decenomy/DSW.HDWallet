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
            logger.LogTrace("Transactions Background Service executing.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var wallets = await storage.GetAllWallets();
                    foreach (var wallet in wallets)
                    {
                        var coinAddresses = await storage.GetAddressesByTicker(wallet.Ticker!);

                        // Ensure addresses is List<string> by excluding nulls
                        var addresses = coinAddresses
                            .Where(ca => ca.Address != null)
                            .Select(ca => ca.Address!)
                            .ToList();

                        var xpub = await blockbookHttpClient.GetXpub(wallet.Ticker!, wallet.PublicKey!);

                        if (xpub.Txids != null)
                        {
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
                                        Type = transactionType,
                                        Amount = transactionAmount,
                                        FromAddress = transactionDetails.Vin != null && transactionDetails.Vin.Any() ? transactionDetails.Vin.First().Addresses?.FirstOrDefault() ?? string.Empty : string.Empty,
                                        ToAddress = transactionDetails.Vout != null && transactionDetails.Vout.Any() ? transactionDetails.Vout.First().Addresses?.FirstOrDefault() ?? string.Empty : string.Empty,
                                        Timestamp = DateTimeOffset.FromUnixTimeSeconds(transactionDetails.BlockTime).UtcDateTime,
                                        IsConfirmed = transactionDetails.Confirmations > 0,
                                        TransactionFee = Convert.ToDecimal(transactionDetails.Fees) / 100000000, // Conversion to standard unit
                                        Notes = ""
                                    };

                                    await storage.AddTransaction(transactionRecord);
                                }
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

            logger.LogTrace("Transactions Background Service executed.");
        }

        private TransactionType DetermineTransactionType(TransactionObject transactionDetails, List<string> walletAddresses)
        {
            bool hasVin = transactionDetails.Vin != null && transactionDetails.Vin.Any();
            bool hasVout = transactionDetails.Vout != null && transactionDetails.Vout.Any();

            Vin? firstVin = null;
            Vout? firstVout = null;

            if (transactionDetails.Vin != null && transactionDetails.Vin.Any())
            {
                firstVin = transactionDetails.Vin.FirstOrDefault();
            }

            if (transactionDetails.Vout != null && transactionDetails.Vout.Any())
            {
                firstVout = transactionDetails.Vout.FirstOrDefault();
            }

            bool firstVinHasValue = firstVin != null && firstVin.Value != null;
            bool firstVoutHasValue = firstVout != null && firstVout.Value != null;

            bool isMining = !hasVin && hasVout && (transactionDetails.Vout ?? Enumerable.Empty<Vout>())
                .Any(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

            bool isStaking = hasVin && transactionDetails.Vin?.Count == 1
                 && hasVout
                 && firstVoutHasValue && firstVout?.Value == "0"
                 && transactionDetails.Vout?.Count > 1 // Ensure there is more than one Vout to safely use Skip(1)
                 && transactionDetails.Vout.Skip(1).Any(vout =>
                     vout.Addresses != null
                     && firstVin?.Addresses != null
                     && vout.Addresses.Contains(firstVin.Addresses.FirstOrDefault() ?? string.Empty))
                 && transactionDetails.Vin.Sum(v => v.Value != null ? Convert.ToDecimal(v.Value) : 0)
                     < transactionDetails.Vout.Sum(v => v.Value != null ? Convert.ToDecimal(v.Value) : 0);

            bool isMasternodeReward = isStaking && hasVout && transactionDetails.Vout?.Any(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr))) == true;

            bool isInternal = hasVin && (transactionDetails.Vin?.All(vin => vin.Addresses != null && vin.Addresses.Any(addr => walletAddresses.Contains(addr))) ?? false)
                && hasVout && (transactionDetails.Vout?.All(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr))) ?? false);

            bool isOutgoing = hasVin && (transactionDetails.Vin?.All(vin => vin.Addresses != null && vin.Addresses.Any(addr => walletAddresses.Contains(addr))) ?? false)
                && hasVout && (transactionDetails.Vout?.Any(vout => vout.Addresses != null && vout.Addresses.All(addr => !walletAddresses.Contains(addr))) ?? false);

            bool isIncoming = hasVout && (transactionDetails.Vout?.Any(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr))) ?? false)
                && (!hasVin || (transactionDetails.Vin?.All(vin => vin.Addresses == null || vin.Addresses.All(addr => !walletAddresses.Contains(addr))) ?? true));

            if (isMining) return TransactionType.Mining;
            if (isMasternodeReward) return TransactionType.MasternodeReward;
            if (isStaking) return TransactionType.Staking;
            if (isInternal) return TransactionType.Internal;
            if (isOutgoing) return TransactionType.Outgoing;
            if (isIncoming) return TransactionType.Incoming;

            return TransactionType.Unknown;
        }

        private long CalculateTransactionAmount(TransactionObject transactionDetails, List<string> walletAddresses, TransactionType transactionType)
        {
            long amount = 0;

            switch (transactionType)
            {
                case TransactionType.Incoming:
                    // Sum of relevant incoming outputs to the wallet addresses
                    amount = transactionDetails.Vout?
                        .Where(vout => vout.Addresses != null && vout.Addresses.Any(addr => walletAddresses.Contains(addr)))
                        .Sum(vout => long.TryParse(vout.Value, out long val) ? val : 0) ?? 0;
                    break;

                case TransactionType.Outgoing:
                    // For outgoing transactions, identify the vout entries sent to external addresses
                    var externalVouts = transactionDetails.Vout?
                        .Where(vout => vout.Addresses == null || !vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

                    // Sum up the values of these vout entries, considering the one with the minimum value as the actual amount sent
                    if (externalVouts != null && externalVouts.Any())
                    {
                        // Assuming the smallest vout value sent to an external address represents the actual transfer amount
                        amount = externalVouts.Min(vout => long.TryParse(vout.Value, out long val) ? val : long.MaxValue);
                    }

                    break;

                case TransactionType.Internal:
                    // No amount change for internal transactions
                    amount = 0;
                    break;

                case TransactionType.Mining:
                case TransactionType.Staking:
                case TransactionType.MasternodeReward:
                    // Sum of all outputs for these transaction types
                    amount = transactionDetails.Vout?
                        .Sum(vout => long.TryParse(vout.Value, out long val) ? val : 0) ?? 0;
                    break;

                default:
                    break;
            }

            return amount;
        }






    }
}
