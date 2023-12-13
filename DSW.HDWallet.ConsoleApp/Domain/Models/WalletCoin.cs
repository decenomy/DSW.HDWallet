namespace DSW.HDWallet.ConsoleApp.Domain.Models
{
    public class WalletCoin
    {
        public string? Ticker { get; set; }
        public string? PublicKey { get; set; }
        public string? Path { get; set; }
        public int CoinIndex { get; set; }
        public int? Balance { get; set; }
    }
}
