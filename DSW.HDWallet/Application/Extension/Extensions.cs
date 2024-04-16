using System.Globalization;

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

        public static decimal ToDecimal(this ulong value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (OverflowException)
            {
                throw new OverflowException("The ulong value is too large to be converted to decimal.");
            }
        }

        public static long ToLong(this string value)
        {
            if (long.TryParse(value, out long result))
            {
                return result;
            }
            throw new FormatException("The input string cannot be converted to a long.");
        }

        public static decimal ToDecimalPoint(this long value)
        {
            try
            {
                string stringValue = value.ToString();
                int length = stringValue.Length;
                string formattedValue;

                if (length == 8)
                {
                    formattedValue = "0." + value.ToString("D8");
                }
                else if (length < 8)
                {
                    formattedValue = "0." + value.ToString("D" + (8 - length)) + value.ToString("D" + length);
                }
                else
                {
                    string integerPart = stringValue.Substring(0, length - 8);
                    string decimalPart = stringValue.Substring(length - 8);

                    formattedValue = integerPart + "." + decimalPart;
                }

                return Convert.ToDecimal(formattedValue, CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                throw new OverflowException("The long value is too large to be converted to decimal.");
            }
            catch (FormatException)
            {
                throw new FormatException("The input value is not a valid long.");
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

        public static string ToFormattedString(this long value)
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
