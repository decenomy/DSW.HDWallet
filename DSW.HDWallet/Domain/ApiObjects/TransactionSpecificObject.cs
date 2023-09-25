namespace DSW.HDWallet.Domain.ApiObjects
{
    public class TransactionSpecificObject
    {
        public string? Hex { get; set; }
        public string? Txid { get; set; }
        public bool Overwintered { get; set; }
        public int Version { get; set; }
        public string? Versiongroupid { get; set; }
        public int Locktime { get; set; }
        public int Expiryheight { get; set; }
        public List<Vin>? Vin { get; set; }
        public List<Vout>? Vout { get; set; }
        public List<object>? Vjoinsplit { get; set; }
        public int ValueBalance { get; set; }
        public List<VShieldedSpend>? VShieldedSpend { get; set; }
        public List<VShieldedOutput>? VShieldedOutput { get; set; }
        public string? BindingSig { get; set; }
        public string? Blockhash { get; set; }
        public int Confirmations { get; set; }
        public int Time { get; set; }
        public int Blocktime { get; set; }
    }
}