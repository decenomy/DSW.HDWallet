using NBitcoin;

namespace DSW.HDWallet.Domain.Utils
{
    public static class SatoshiConverter
    {
        private const decimal SubSatoshiFactor = 10_000_000_000m;  // 10^10
        private const decimal SatoshiFactor = 100_000_000m;  // 10^8

        public static long ToSubSatoshi(decimal value)
        {
            return (long)(value * SubSatoshiFactor);
        }

        public static decimal FromSubSatoshi(long satoshiValue)
        {
            return satoshiValue / SubSatoshiFactor;
        }

        public static long ToSatoshi(decimal value)
        {
            return (long)(value * SatoshiFactor);
        }

        public static decimal FromSatoshi(long satoshiValue)
        {
            return satoshiValue / SatoshiFactor;
        }
    }
}
