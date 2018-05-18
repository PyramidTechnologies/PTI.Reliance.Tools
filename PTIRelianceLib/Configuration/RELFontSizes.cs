#region Header
// RELFontSizes.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:27 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    /// <summary>
    /// Describes available font sizes by ID. These must match firmware values.
    /// </summary>
    internal enum RelianceFontSizes : byte
    {
        /// <summary>
        /// A CPI 11, B CPI 15
        /// </summary>
        A11_B15 = 0,
        /// <summary>
        /// A CPI 15, B CPI 20
        /// </summary>
        A15_B20 = 1,
        /// <summary>
        /// A CPI 120, B CPI 15
        /// </summary>
        A20_B15 = 2,

        Unset = 0xFF
    }
}