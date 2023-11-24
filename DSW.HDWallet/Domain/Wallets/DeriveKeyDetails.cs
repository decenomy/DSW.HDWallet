namespace DSW.HDWallet.Domain.Wallets
{
    public class DeriveKeyDetails
    {
        public required string PubKey{ get; set; }
        public required string Address { get; set; }
        public required string Path { get; set; }
    }

    public class AddressInfo
    {
        public required string Address { get; set; }
        public required string Path { get; set; }
    }

}
