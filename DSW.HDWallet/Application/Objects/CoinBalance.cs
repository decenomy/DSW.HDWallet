using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Application.Objects
{
    public class CoinBalance
    {
        public decimal Balance { get; set; }
        public decimal UnconfirmedBalance { get; set; }
        public decimal LockedBalance { get; set; }
        public decimal TotalBalance { get; set; }

        public CoinBalance(decimal balance, decimal unconfirmedBalance, decimal lockedBalance)
        {
            Balance = balance;
            UnconfirmedBalance = unconfirmedBalance;
            LockedBalance = lockedBalance;
            TotalBalance = balance + unconfirmedBalance;
        }
    }
}
