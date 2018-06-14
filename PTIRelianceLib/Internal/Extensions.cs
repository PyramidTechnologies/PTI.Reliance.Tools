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
    using System.IO;
    using System.Runtime.Serialization;
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
            return (ushort) ((b[1] << 8) | b[0]);
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
        /// Converts an unsigned int to a big Endian 2-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this ushort u)
        {
            return new[] { (byte)(u & 0xFF), (byte)((u >> 8) & 0xFF) };
        }

        /// <summary>
        /// Converts a signed int to a big Endian 4-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this int u)
        {
            return new[] { (byte)(u & 0xFF), (byte)((u >> 8) & 0xFF), (byte)((u >> 16) & 0xFF), (byte)((u >> 24) & 0xFF) };
        }

        /// <summary>
        /// Converts an unsigned int to a big Endian 4-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this uint u)
        {
            return new[] { (byte)(u & 0xFF), (byte)((u >> 8) & 0xFF), (byte)((u >> 16) & 0xFF), (byte)((u >> 24) & 0xFF) };
        }

        /// <summary>
        /// Converts a signed long to a big Endian 8-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this long u)
        {
            return new[]
            {
                (byte)(u & 0xFF), (byte)((u >> 8) & 0xFF), (byte)((u >> 16) & 0xFF), (byte)((u >> 24) & 0xFF),
                (byte)((u >> 32) & 0xFF), (byte)((u >> 40) & 0xFF), (byte)((u >> 48) & 0xFF), (byte)((u >> 56) & 0xFF)
            };
        }

        /// <summary>
        /// Converts an unsigned long to a big Endian 8-byte array
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static byte[] ToBytesBE(this ulong u)
        {
            return new[]
            {
                (byte)(u & 0xFF), (byte)((u >> 8) & 0xFF), (byte)((u >> 16) & 0xFF), (byte)((u >> 24) & 0xFF),
                (byte)((u >> 32) & 0xFF), (byte)((u >> 40) & 0xFF), (byte)((u >> 48) & 0xFF), (byte)((u >> 56) & 0xFF)
            };
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


        /// <summary>
        /// Concatentates all arrays into one
        /// </summary>
        /// <param name="args">1 or more byte arrays</param>
        /// <returns>byte[]</returns>
        public static byte[] Concat(params byte[][] args)
        {
            using (var buffer = new MemoryStream())
            {
                foreach (var ba in args)
                {
                    buffer.Write(ba, 0, ba.Length);
                }

                var bytes = new byte[buffer.Length];
                buffer.Position = 0;
                buffer.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }


        /// <summary>
        /// Gets the EnumMemberAttribute on an enum field value. If the attribute
        /// is not set, the ToString() result will be returned instead.
        /// </summary>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The string form of this enumVal</returns>
        public static string GetEnumName(this Enum enumVal)
        {
            if (enumVal == null)
            {
                return string.Empty;
            }


            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length == 0)
            {
                return string.Empty;
            }

            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
            return attributes.Length > 0 && attributes[0] != null
                ? ((EnumMemberAttribute)attributes[0]).Value
                : enumVal.ToString();
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i">Value to round</param>
        /// <param name="N">Multiple to round to</param>
        /// <returns></returns>
        public static int RoundUp(this int i, int N)
        {
            return (int)RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i">Value to round</param>
        /// <param name="N">Multiple to round to</param>
        /// <returns></returns>
        public static long RoundUp(this long i, int N)
        {
            return RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i">Value to round</param>
        /// <param name="N">Multiple to round to</param>
        /// <returns></returns>
        public static uint RoundUp(this uint i, int N)
        {
            return (uint)RoundUp(i, (uint)N);
        }


        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i">Value to round</param>
        /// <param name="N">Multiple to round to</param>
        /// <returns></returns>
        private static long RoundUp(long i, uint N)
        {
            if (N == 0)
            {
                return 0;
            }

            if (i == 0)
            {
                return N;
            }

            return (long)(Math.Ceiling(Math.Abs(i) / (double)N) * N);
        }
    }
}
