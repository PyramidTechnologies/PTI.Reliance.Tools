#region Header

// ImageTestHelpers.cs
// PTIRelianceLib.Tests
// Cory Todd
// 13-06-2018
// 2:00 PM

#endregion

namespace PTIRelianceLib.Tests.Imaging
{
    using System;
    using System.Drawing;
    using System.Linq;
    using PTIRelianceLib.Imaging;

    public enum ImageConvertResults
    {
        Success,
        ErrToBuffer,
        ErrToBitmap,
    }

    public static class ImageTestHelpers
    {
        public static ImageConvertResults TestBitmapConversion(Bitmap bmp, byte[] expectedBuff)
        {
            var actualBuff = bmp.ToBuffer();
            if (!expectedBuff.SequenceEqual(actualBuff))
            {
                return ImageConvertResults.ErrToBuffer;
            }


            // Now convert back to bitmap
            var expectedBmp = new Bitmap(bmp);
            var actualBmp = actualBuff.AsBitmap(expectedBmp.Width, expectedBmp.Height);

            return !CompareCrc32(expectedBmp, actualBmp)
                ? ImageConvertResults.ErrToBitmap
                : ImageConvertResults.Success;
        }

        /// <summary>
        /// Compare checksum of two bitmaps to verify equality
        /// </summary>
        /// <param name="b1">First image to compare</param>
        /// <param name="b2">Second image to comare</param>
        /// <returns>True if images are equal</returns>
        public static bool CompareCrc32(Bitmap b1, Bitmap b2)
        {
            // both null, treat nulls as equal
            if (b1 == null && b2 == null)
            {
                return true;
            }

            // One or the other is null, this is not equal
            if (b1 == null || b2 == null)
            {
                return false;
            }
            
            if (!b1.Size.Equals(b2.Size))
            {
                return false;
            }

            var left = b1.ToBuffer();
            var right = b1.ToBuffer();

            return (Crc32.ComputeChecksum(left) == Crc32.ComputeChecksum(right));
        }

        /// <summary>
        /// Fills a repeated BGRA pattern into a buffer of count bytes. Byte order is
        /// Blue, Green, Red, Alpha
        /// </summary>
        /// <param name="pattern">4-byte BGRA code</param>
        /// <param name="count">Number of times to repeat pattern</param>
        /// <returns>new buffer</returns>
        public static byte[] BgraGenerator(byte[] pattern, int count)
        {
            if (pattern.Length != 4)
            {
                throw new ArgumentException("pattern length must be 4");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count be greater than zero");
            }

            var result = new byte[pattern.Length * count];

            for (var i = 0; i < result.Length; i += 4)
            {
                Array.Copy(pattern, 0, result, i, 4);
            }

            return result;
        }
    }
}