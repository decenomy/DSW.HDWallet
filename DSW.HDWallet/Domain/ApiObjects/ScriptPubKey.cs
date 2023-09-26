namespace DSW.HDWallet.Domain.ApiObjects
{
    public class ScriptPubKey
    {
        public string? Asm { get; set; }
        public string? Hex { get; set; }
        public string? Type { get; set; }
        public int? ReqSigs { get; set; }
        public List<string>? Addresses { get; set; }
    }
}
