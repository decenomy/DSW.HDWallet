namespace DSW.HDWallet.Domain.Models
{
    public class Rate
    {
        public string? TickerFrom { get; set; }
        public string? TickerTo { get; set; }
        public long RateValue { get; set; }
    }
}
