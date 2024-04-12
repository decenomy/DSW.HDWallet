using DSW.HDWallet.Application;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSW.HDWallet.Infrastructure.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IStorage storage;
        private readonly IWalletService walletService;
        private readonly ILogger<BalanceService> logger;

        public BalanceService(IStorage storage, IWalletService walletService, ILogger<BalanceService> logger)
        {
            this.storage = storage;
            this.walletService = walletService;
            this.logger = logger;
        }

        public async Task UpdateAllBalancesAsync()
        {
            var walletCoins = await storage.GetAllWallets();
            foreach (var coin in walletCoins)
            {
                if (coin.Ticker != null && coin.PublicKey != null)
                {
                    try
                    {
                        var balance = await walletService.GetXpub(coin.Ticker, coin.PublicKey, 1, 1);

                        if (long.TryParse(balance.Balance, out long balanceValue))
                        {
                            decimal realBalance = SatoshiConverter.FromSatoshi(balanceValue);
                            coin.Balance = SatoshiConverter.ToSubSatoshi(realBalance);

                            if (long.TryParse(balance.UnconfirmedBalance, out long unconfirmedBalanceValue))
                            {
                                decimal realUnconfirmedBalance = SatoshiConverter.FromSatoshi(unconfirmedBalanceValue);
                                coin.UnconfirmedBalance = SatoshiConverter.ToSubSatoshi(realUnconfirmedBalance);
                            }
                            await storage.SaveBalance(coin);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error updating balance for {coin.Ticker}: {ex.Message}");
                    }
                }
            }
        }
    }
}
