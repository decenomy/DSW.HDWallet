using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Models;
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
                                    Type = transactionType, 
                                    Amount = transactionAmount, 
                                    FromAddress = transactionDetails.Vin.FirstOrDefault()?.Addresses.FirstOrDefault(),
                                    ToAddress = transactionDetails.Vout.FirstOrDefault()?.Addresses.FirstOrDefault(),
                                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(transactionDetails.BlockTime).UtcDateTime,
                                    IsConfirmed = transactionDetails.Confirmations > 0,
                                    TransactionFee = Convert.ToDecimal(transactionDetails.Fees) / 100000000, // Example conversion
                                    Notes = "" // Any additional information or leave empty
                                };

                                //await storage.AddTransaction(transactionRecord);
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

        private string DetermineTransactionType(TransactionObject transactionDetails, List<string> walletAddresses)
        {
            // Mining transaction: no Vins and at least one Vout with an address from my wallet
            bool isMining = !transactionDetails.Vin.Any() && transactionDetails.Vout.Any(vout => vout.Addresses.Any(addr => walletAddresses.Contains(addr)));

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

            if (isMining)
                return "Mining";
            if (isMasternodeReward)
                return "Masternode Reward";
            if (isStaking)
                return "Staking";
            if (isInternal)
                return "Internal";
            if (isOutgoing)
                return "Outgoing";
            if (isIncoming)
                return "Incoming";

            return "Unknown";
        }

        private decimal CalculateTransactionAmount(TransactionObject transactionDetails, List<string> walletAddresses, string transactionType)
        {
            decimal amount = 0;

            if (transactionType == "Incoming")
            {
                foreach (var vout in transactionDetails.Vout)
                {
                    if (vout.Addresses.Any(addr => walletAddresses.Contains(addr)))
                    {
                        amount += Convert.ToDecimal(vout.Value);
                    }
                }
            }
            else if (transactionType == "Outgoing")
            {
                foreach (var vin in transactionDetails.Vin)
                {
                    if (vin.Addresses.Any(addr => walletAddresses.Contains(addr)))
                    {
                        amount += Convert.ToDecimal(vin.Value);
                    }
                }

                // Subtracting the transaction fee from the total amount for outgoing transactions
                amount -= Convert.ToDecimal(transactionDetails.Fees);
            }

            return amount / 100000000; // TODO Convert from Satoshi
        }

    }
}