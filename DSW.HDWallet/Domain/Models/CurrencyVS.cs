namespace DSW.HDWallet.Domain.Models
{
    public class CurrencyVS
    {
        public List<string> GetCurrencyTickers()
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
