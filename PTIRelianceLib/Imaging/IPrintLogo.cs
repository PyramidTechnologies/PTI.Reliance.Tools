#region Header

// IPrintLogo.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:18 PM

#endregion

namespace PTIRelianceLib.Imaging
{
    using System.Drawing;

    /// <summary>
    /// Constract defines an image format that supports standard
    /// transforms and manipulations
    /// </summary>
    internal interface IPrintLogo
    {
        /// <summary>
        /// Apply a dithering ditherAlgorithm
        /// </summary>
        /// <param name="ditherAlgorithm">Algorithm</param>
        /// <param name="threshhold">Pixel on/off threshhold</param>
        void ApplyDithering(DitherAlgorithms ditherAlgorithm, byte threshhold);

        /// <summary>
        /// Returns image as raw array with correct bit ordering
        /// for transmission to target. If data is not available
        /// an empty buffer will be returned.
        /// </summary>
        /// <returns>byte[]</returns>
        byte[] ToBuffer();

        /// <summary>
        /// Returns the dimensions of this image
        /// </summary>
        LogoSize Dimensions { get; }

        /// <summary>
        /// Returns the ideal height in pixels that was specified
        /// </summary>
        int IdealHeight { get; }

        /// <summary>
        /// Returnst the ideal width in pixels that was specified
        /// </summary>
        int IdealWidth { get; }

        /// <summary>
        /// Returns the maximum height that was specified when this image was created
        /// </summary>
        int MaxHeight { get; }

        /// <summary>
        /// Returns the maximum width that was specified when this image was crearted
        /// </summary>
        int MaxWidth { get; }

        /// <summary>
        /// Backing image data
        /// </summary>
        Bitmap ImageData { get; }
    }
}