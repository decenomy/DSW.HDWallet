
using DSW.HDWallet.Domain.Models;

namespace DSW.HDWallet.Domain.Utils
{
    public static class FiatCalculator
    {

        public static decimal GetFiatValue(long amount, Rate rate)
        {
            decimal decimalAmount = SatoshiConverter.FromSubSatoshi(amount);
            decimal decimalRateValue = SatoshiConverter.FromSatoshi(rate.RateValue);

            // Perform the conversion
            decimal fiatValue = decimalAmount * decimalRateValue;

            return fiatValue;
        }

    }
}

