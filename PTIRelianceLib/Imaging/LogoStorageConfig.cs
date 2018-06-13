#region Header
// LogoStorageConfig.cs
// PTIRelianceLib
// Cory Todd
// 14-06-2018
// 10:05 AM
#endregion

namespace PTIRelianceLib.Imaging
{
    /// <summary>
    /// Storage configuration for Reliance logos
    /// </summary>
    public class LogoStorageConfig
    {
        /// <summary>
        /// Gets or Sets the maximum width in pixels for images being stored.
        /// Any images with a width greater than this value will be scaled down
        /// proportionally. For best result, it is recommended that you provide
        /// images at your desired size so you have better control over scaling.
        /// </summary>
        public int MaxPixelWidth { get; set; } = 640;

        /// <summary>
        /// Dithering threshold value determines at which magitude a pixel
        /// swithes from black to white or white to black.
        /// </summary>
        public byte Threshold { get; set; } = 127;

        /// <summary>
        /// Dithering implementation to utilize for image
        /// </summary>
        public DitherAlgorithms Algorithm { get; set; } = DitherAlgorithms.None;

        /// <summary>
        /// Returns default logo storage options
        /// </summary>
        public static LogoStorageConfig Default => new LogoStorageConfig();
    }
}