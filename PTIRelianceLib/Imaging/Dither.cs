#region Header

// Dither.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:22 PM

#endregion

namespace PTIRelianceLib.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Base dithering class
    /// </summary>
    internal class Dither : IDitherable
    {
        // We do not use jagged arrays because I feel that syntax of multi-dimensional
        // arrays is easier to work with  
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private readonly byte[,] _mMatrixPattern;

        private readonly bool _mCanShift;
        private readonly int _mMatrixOffset;

        /// <summary>
        /// Creates an instance of this dithering class
        /// </summary>
        /// <param name="matrixPattern">algorithm in matrix form</param>
        /// <param name="divisor">algorithm divisor</param>
        /// <param name="threshold">threshhold threshold at which a pixel is considered 'black'</param>
        /// <param name="shift">True to enable use of logical shift instead of multiply operation</param>
        /// <exception cref="ArgumentNullException">Thrown if matrixPattern is null</exception>
        /// <exception cref="ArgumentException">Thrown if divisor is zero</exception>
        public Dither(byte[,] matrixPattern, int divisor, byte threshold, bool shift = false)
        {
            if (divisor == 0)
            {
                throw new ArgumentException("divisor must be non-zero");
            }            

            _mMatrixPattern = matrixPattern ?? throw new ArgumentNullException(nameof(matrixPattern));
            RowCount = matrixPattern.GetUpperBound(0) + 1;
            ColCount = matrixPattern.GetUpperBound(1) + 1;

            Divisor = divisor;
            Threshold = threshold;

            _mCanShift = shift;

            // Find first non-zero coefficient column in matrix. This value must
            // always be in the first row of the matrix
            for (var i = 0; i < ColCount; i++)
            {
                if (matrixPattern[0, i] == 0)
                {
                    continue;
                }

                _mMatrixOffset = (byte) (i - 1);
                break;
            }
        }

        /// <inheritdoc />
        public int RowCount { get; }

        /// <inheritdoc />
        public int ColCount { get; }

        /// <inheritdoc />
        public int Divisor { get; }

        /// <inheritdoc />
        public byte Threshold { get; }

        /// <inheritdoc />
        public virtual Bitmap GenerateDithered(Bitmap bitmap)
        {
            var bmpBuff = bitmap.ToBuffer();
            var pixels = bmpBuff.Split(4).Select(pix => new Pixel(pix)).ToList();

            // Convert all bytes into pixels

            // Dither away
            for (var x = 0; x < bitmap.Height; x++)
            {
                for (var y = 0; y < bitmap.Width; y++)
                {
                    var index = x * bitmap.Width + y;
                    var colored = pixels[index];
                    var grayed = ApplyGrayscale(colored);
                    pixels[index] = grayed;

                    ApplySmoothing(pixels, colored, grayed, y, x, bitmap.Width, bitmap.Height);
                }
            }

            // Dump results into output
            var output = new byte[pixels.Count << 2];
            var j = 0;
            foreach (var p in pixels)
            {
                output[j++] = p.B;
                output[j++] = p.G;
                output[j++] = p.R;

                // RT-15 - force alpha to be 0xFF because in optimized mode,
                // the .NET client may send strange bitmap data.
                output[j++] = 0xFF;
            }

            return output.AsBitmap(bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Apply grayscale to this pixel and return result
        /// </summary>
        /// <param name="pix">Pixel to transform</param>
        /// <returns>color reduced (grayscale) pixel</returns>
        protected virtual Pixel ApplyGrayscale(Pixel pix)
        {
            // Magic numbers for converting RGB to monochrome space. These achieve a balanced grayscale
            var grayPoint = (byte) (0.299 * pix.R + 0.587 * pix.G + 0.114 * pix.B);

            // Do not alter the alpha channel, otherwise the entire image may go opaque
            Pixel grayed;
            grayed.A = pix.A;

            if (grayPoint < Threshold)
            {
                grayed.R = grayed.G = grayed.B = 0;
            }
            else
            {
                grayed.R = grayed.G = grayed.B = 255;
            }

            return grayed;
        }

        /// <summary>
        /// Apply Dithering algorithm
        /// </summary>
        /// <param name="imageData">image in row-major order to dither against</param>
        /// <param name="colored">Pixel source</param>
        /// <param name="grayed">Pixel source</param>
        /// <param name="x">column position of Pixel</param>
        /// <param name="y">y row position of Pixel</param>
        /// <param name="width">width of imageData</param>
        /// <param name="height">height of imageData</param>
        protected virtual void ApplySmoothing(
            IList<Pixel> imageData,
            Pixel colored,
            Pixel grayed,
            int x,
            int y,
            int width,
            int height)
        {
            var redError = colored.R - grayed.R;
            var blueError = colored.G - grayed.G;
            var greenError = colored.B - grayed.B;

            for (var row = 0; row < RowCount; row++)
            {
                // Convert row to row-major index
                var ypos = y + row;

                for (var col = 0; col < ColCount; col++)
                {
                    int coefficient = _mMatrixPattern[row, col];

                    // Convert col to row-major index
                    var xpos = x + (col - _mMatrixOffset);

                    // Do not process outside of image, 1st row/col, or if pixel is 0
                    if (coefficient == 0 || xpos <= 0 || xpos >= width || ypos <= 0 || ypos >= height)
                    {
                        continue;
                    }

                    var offset = ypos * width + xpos;
                    var dithered = imageData[offset];

                    int newR, newG, newB;

                    // Calculate the dither effect on each color channel
                    if (_mCanShift)
                    {
                        newR = (redError * coefficient) >> Divisor;
                        newG = (greenError * coefficient) >> Divisor;
                        newB = (blueError * coefficient) >> Divisor;
                    }
                    else
                    {
                        newR = (redError * coefficient) / Divisor;
                        newG = (greenError * coefficient) / Divisor;
                        newB = (blueError * coefficient) / Divisor;
                    }

                    // Be sure not to overflow
                    dithered.R = SafeByteCast(dithered.R + newR);
                    dithered.G = SafeByteCast(dithered.G + newG);
                    dithered.B = SafeByteCast(dithered.B + newB);

                    // Apply new color
                    imageData[offset] = dithered;
                }
            }
        }

        /// <summary>
        /// Returns a integer as a byte and handle any over/underflow
        /// </summary>
        /// <param name="val">int</param>
        /// <returns>byte</returns>
        private static byte SafeByteCast(int val)
        {
            if (val < 0)
            {
                val = 0;
            }
            else if (val > 255)
            {
                val = 255;
            }

            return (byte) val;
        }
    }
}