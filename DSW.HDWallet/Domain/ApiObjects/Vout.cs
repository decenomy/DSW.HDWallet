namespace DSW.HDWallet.Domain.ApiObjects
{
    public class Vout
    {
        public string? Value { get; set; }
        public int N { get; set; }
        public List<string>? Addresses { get; set; }
        public bool? IsAddress { get; set; }
        public string? Hex { get; set; }               
    }
}
