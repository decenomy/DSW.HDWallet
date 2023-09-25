namespace DSW.HDWallet.Domain.ApiObjects
{
    public class VShieldedOutput
    {
        public string? cv { get; set; }
        public string? cmu { get; set; }
        public string? ephemeralKey { get; set; }
        public string? encCiphertext { get; set; }
        public string? outCiphertext { get; set; }
        public string? proof { get; set; }
    }
}