#region Header
// DitherAlgorithms.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:20 PM
#endregion

namespace PTIRelianceLib.Imaging
{
    using System.Runtime.Serialization;

    /// <summary>
    /// List of available dithering algorithms
    /// </summary>
    public enum DitherAlgorithms
    {
        /// <summary>
        /// Dither algorithm reduces color depth but maintains detail accuracy
        /// </summary>
        [EnumMember(Value = "Dia")]
        JarvisJudiceNinke,
        /// <summary>
        /// Better color deptch, lower accuracy
        /// </summary>
        [EnumMember(Value = "Metis")]
        FloydSteinberg,
        /// <summary>
        /// Simple dither
        /// </summary>
        [EnumMember(Value = "Io")]
        Atkinson,
        /// <summary>
        /// Simple dither
        /// </summary>
        [EnumMember(Value = "Leda")]
        Stucki,
        /// <summary>
        /// Raw image
        /// </summary>
        [EnumMember(Value = "Adrastea")]
        None,
        /// <summary>
        /// Simple dither
        /// </summary>
        [EnumMember(Value = "Carpo")]
        FloydSteinbergFalse,
        /// <summary>
        /// Decent dithering
        /// </summary>
        [EnumMember(Value = "Europa")]
        Sierra,
        /// <summary>
        /// Slightly better Sierra kernel
        /// </summary>
        [EnumMember(Value = "Thebe")]
        Sierra2,
        /// <summary>
        /// Fast Sierra kernel
        /// </summary>
        [EnumMember(Value = "Ganymede")]
        SierraLite,
        /// <summary>
        /// Decent dithering
        /// </summary>
        [EnumMember(Value = "Callisto")]
        Burkes,
    }
}