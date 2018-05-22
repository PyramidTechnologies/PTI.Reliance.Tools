#region Header
// RELScalarModes.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:26 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Scaling mode affects the global scaling factor applied
    /// to all printed glyphs.
    /// </summary>
    internal enum RelianceScalarMode
    {
        /// <summary>
        /// Emulates the original font size by applying a scalar of 1.0f
        /// </summary>
        [EnumMember(Value = "Legacy")]
        Legacy = 0,
        /// <summary>
        /// New style scaling applies a fixed "ideal" scale factor for larger font
        /// </summary>
        [EnumMember(Value = "Ideal")]
        Ideal = 1
    }
}