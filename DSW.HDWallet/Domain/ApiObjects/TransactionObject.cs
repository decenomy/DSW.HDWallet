namespace DSW.HDWallet.Domain.ApiObjects
{
    public class TransactionObject
    {
        public string? Txid { get; set; }
        public int Version { get; set; }
        public List<Vin>? Vin { get; set; }
        public List<Vout>? Vout { get; set; }
        public string? BlockHash { get; set; }
        public int BlockHeight { get; set; }
        public int Confirmations { get; set; }
        public int BlockTime { get; set; }
        public int Size { get; set; }
        public string? Value { get; set; }
        public string? ValueIn { get; set; }
        public string? Fees { get; set; }
        public string? Hex { get; set; }
    }
}