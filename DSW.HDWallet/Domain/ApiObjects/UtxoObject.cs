namespace DSW.HDWallet.Domain.ApiObjects
{
    public class UtxoObject
    {
        public string? Txid { get; set; }
        public int Vout { get; set; }
        public string? Value { get; set; }
        public int Height { get; set; }
        public int Confirmations { get; set; }
    }
}
