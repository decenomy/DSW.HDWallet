namespace DSW.HDWallet.Domain.WSObject
{
    public class WSRequest
    {
        public string? method { get; set; }
        public Params? @params { get; set; }
        public string? flag { get; set; }
    }

    public class Params
    {
        public string? txid { get; set; }
    }
}
