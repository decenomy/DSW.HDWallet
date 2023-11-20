using DSW.HDWallet.Domain.Coins;

namespace DSW.HDWallet.Domain.Wallets
{
    public class PubKeyDetails
    {
        public required string PubKey { get; set; }
        public required string Ticker { get; set; }
        public required int Index { get; set; }
        public required string Path { get; set; }

    }
}
