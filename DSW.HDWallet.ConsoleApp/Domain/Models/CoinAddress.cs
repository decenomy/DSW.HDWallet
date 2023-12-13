namespace DSW.HDWallet.ConsoleApp.Domain.Models
{
    public class CoinAddress
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public string Address { get; set; }
        public int AddressIndex { get; set; }
        public bool IsUsed { get; set; }
        public bool IsChange { get; set; }
    }
}
