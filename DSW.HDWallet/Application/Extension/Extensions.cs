namespace DSW.HDWallet.Application.Extension
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
