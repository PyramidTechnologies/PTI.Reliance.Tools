#region Header

// LogoSize.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:20 PM

#endregion

namespace PTIRelianceLib.Imaging
{
    internal class LogoSize
    {
        /// <summary>
        /// Width of image in bytes
        /// </summary>
        public int WidthBytes { get; set; }

        /// <summary>
        /// Width of image in thermal printer dots (def. 203 DPI)
        /// </summary>
        public int WidthDots { get; set; }

        /// <summary>
        /// Height of image in dots (and bytes, they're the same metric here)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes for this logo
        /// </summary>
        public int SizeInBytes { get; set; }

        /// <summary>
        /// Returns dimension string as WidthxHeight
        /// </summary>
        /// <returns></returns>
        public object GetBitmapSizeString()
        {
            return string.Format("{0}x{1}", WidthDots, Height);
        }
    }
}