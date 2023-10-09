namespace DSW.HDWallet.Application.Extension
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public static ulong ToULong(this string input)
        {
            if (ulong.TryParse(input, out ulong result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("The string cannot be converted to ulong.");
            }
        }

        public static string ToFormattedString(this ulong value)
        {
            string stringValue = value.ToString();
            if (stringValue.Length > 8)
            {
                int decimalPosition = stringValue.Length - 8;
                return stringValue.Substring(0, decimalPosition) + "." + stringValue.Substring(decimalPosition);
            }
            else
            {
                return "0." + stringValue.PadLeft(8, '0');
            }
        }

        public static string ToFormattedString(this string value)
        {
            if (value.Length > 8)
            {
                int decimalPosition = value.Length - 8;
                return value.Substring(0, decimalPosition) + "." + value.Substring(decimalPosition);
            }
            else
            {
                return "0." + value.PadLeft(8, '0');
            }
        }
    }
}
