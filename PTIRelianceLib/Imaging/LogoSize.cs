﻿#region Header

// LogoSize.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:20 PM

#endregion

namespace PTIRelianceLib.Imaging
{
    /// <summary>
    /// Describes the dimensions of a logo in both dots (pixels) and bytes
    /// </summary>
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
    }
}