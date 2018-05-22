#region Header
// RELPaperSizes.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:26 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reliance paper sizes can be any value from 48 to 80
    /// These are predefined for convenience
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum PaperSizes : byte
    {
        /// <summary>
        /// Standard 80mm roll
        /// </summary>
        [EnumMember(Value = "80mm")]
        Roll80Mm = 80,

        /// <summary>
        /// Unusual size does not actually exist
        /// </summary>
        [EnumMember(Value = "60mm")]
        Roll60Mm = 60,

        /// <summary>
        /// What you get when you order a 60mm roll
        /// </summary>
        [EnumMember(Value = "58mm")]
        Roll58Mm = 58,
    }

    /// <summary>
    /// Helpers for paper size
    /// </summary>
    internal static class PaperSizeUtils
    {
        /// <summary>
        /// Convert value to enum
        /// </summary>
        /// <param name="b">value</param>
        /// <returns></returns>
        public static PaperSizes FromByte(byte b)
        {
            switch (b)
            {
                case 58:
                    return PaperSizes.Roll58Mm;
                case 60:
                    return PaperSizes.Roll60Mm;
                default:
                    return PaperSizes.Roll80Mm;
            }
        }

        /// <summary>
        /// Convert paper size to int value
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static byte ToByte(PaperSizes p)
        {
            switch (p)
            {
                case PaperSizes.Roll58Mm:
                    return 58;
                case PaperSizes.Roll60Mm:
                    return 60;
                default:
                    return 80;
            }
        }
    }
}