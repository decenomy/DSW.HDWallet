namespace DSW.HDWallet.Domain.Models
{
    public static class CurrencyVS
    {
        public static List<string> GetCurrencyTickers()
        {
            return new List<string>
            {
                "usd",
                "eur",
                "btc"
            };
        }
    }
}
