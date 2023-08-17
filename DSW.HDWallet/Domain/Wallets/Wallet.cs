namespace DSW.HDWallet.Domain.Wallets
{
    public class Wallet
    {
        public string? MasterKey { get; set; }
        public string? Address { get; set; }
        public string? SecretWords { get; set; }
        public string[]? SecrectWordsArray { get; set; }
    }
}
