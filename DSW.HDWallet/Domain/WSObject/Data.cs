using DSW.HDWallet.Domain.ApiObjects;

namespace DSW.HDWallet.Domain.WSObject
{
    public class Data
    {
        public string txid { get; set; }
        public int version { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public string blockHash { get; set; }
        public int blockHeight { get; set; }
        public int confirmations { get; set; }
        public int blockTime { get; set; }
        public int size { get; set; }
        public string value { get; set; }
        public string valueIn { get; set; }
        public string fees { get; set; }
        public string hex { get; set; }
    }
}
