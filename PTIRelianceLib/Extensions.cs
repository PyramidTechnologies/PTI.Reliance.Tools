#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:07 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.Text;
    
    internal static class Extensions
    {
        /// <summary>
        /// Converts a big endian byte array of bytes to an unsiged word (short)
        /// </summary>
        /// <param name="b">byte array</param>
        /// <returns>short big endian</returns>
        public static ushort ToUshortBE(this byte[] b)
        {
            return (ushort) ((b[1] << 8) | (b[0]));
        }

        /// <summary>
        /// Converts a byte[] to a big endian unsigned integer 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static uint ToUintBE(this byte[] bytes)
        {
            switch (bytes.Length)
            {
                case 4:
                    return (uint) (bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0]);
                case 3:
                    return (uint) (bytes[2] << 16 | bytes[1] << 8 | bytes[0]);
                case 2:
                    return (uint) (bytes[1] << 8 | bytes[0]);
                case 1:
                    return bytes[0];
                default:
                    throw new ArgumentException(
                        $"Bytes must be between 1 and 4 bytes in length, received: {bytes.Length}");

            }
        }

        /// <summary>
        /// Converts an unsigned int to a big Endian 4-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this uint u)
        {
            return new[] { ((byte)((u) & 0xFF)), (byte)((u >> 8) & 0xFF), ((byte)((u >> 16) & 0xFF)), ((byte)((u >> 24) & 0xFF)) };
        }

        /// <summary>
        /// Returns a copy of this string containing only printable
        /// ASCII characters (32-126)
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>Copy of string</returns>
        public static string StripNonPrintableChars(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var c in s)
            {
                if (c >= 32 && c < 127)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns a string from the default text encoding
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetPrintableString(this byte[] bytes)
        {
            return Encoding.GetEncoding(0).GetString(bytes).StripNonPrintableChars();
        }

        /// <summary>
        /// Returns the data formatted as specifed by the format string.
        /// </summary>
        /// <param name="data">byte[]</param>
        /// <param name="delimeter">delimiter such as command or tab</param>
        /// <param name="hexPrefix">Set true to prefix each byte with 0x</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(this byte[] data, string delimeter = ", ", bool hexPrefix = false)
        {
            var hex = new StringBuilder(data.Length * 2);

            var prefix = string.Empty;
            if (hexPrefix)
            {
                prefix = "0x";
            }

            foreach (var b in data)
            {
                hex.AppendFormat("{0}{1:X2}{2}", prefix, b, delimeter);
            }
            var result = hex.ToString().Trim().TrimEnd(delimeter.ToCharArray());
            return result;
        }

        /// <summary>
        /// Split the given array into x number of smaller arrays, each of length len
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayIn"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static T[][] Split<T>(this T[] arrayIn, int len)
        {
            var even = arrayIn.Length % len == 0;
            var totalLength = arrayIn.Length / len;
            if (!even)
            {
                totalLength++;
            }

            var newArray = new T[totalLength][];
            for (var i = 0; i < totalLength; ++i)
            {
                var allocLength = len;
                if (!even && i == totalLength - 1)
                {
                    allocLength = arrayIn.Length % len;
                }

                newArray[i] = new T[allocLength];
                Array.Copy(arrayIn, i * len, newArray[i], 0, allocLength);
            }

            return newArray;
        }
    }
}
