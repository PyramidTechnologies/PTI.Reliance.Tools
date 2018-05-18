#region Header
// RELPrintQuality.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:24 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Encapsulates various firmware parameters to achieve
    /// one of the following settings
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum ReliancePrintQuality
    {
        /// <summary>
        /// A balanced performance of speed and quality
        /// </summary>      
        [EnumMember(Value = "Normal")] Normal = 0,

        /// <summary>
        /// Sacrified speed for quality
        /// </summary>
        [EnumMember(Value = "High Quality")] HighQuality = 1,

        /// <summary>
        /// Sacrifice quality for speed
        /// </summary>
        [EnumMember(Value = "High Speed")] HighSpeed = 2,


        Invalid = 0xFF
    }
}