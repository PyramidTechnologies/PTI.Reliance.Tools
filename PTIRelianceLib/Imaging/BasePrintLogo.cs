#region Header
// BasePrintLogo.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:21 PM
#endregion

namespace PTIRelianceLib.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Logging;

    /// <inheritdoc />
    /// <summary>
    /// A ditherable and resizeable image
    /// </summary>
    internal class BasePrintLogo : IPrintLogo
    {
        private static readonly ILog Log = LogProvider.For<BasePrintLogo>();

        /// <summary>
        /// Construct a new logo from source image and scale to ratio.
        /// Set maxWidthPixels to 0 for full size (no change). If only width or only height
        /// are provided, the image will be scaled proportionall. If both width and height
        /// are provided, the scale will done according to your parameters.
        /// </summary>
        /// <param name="file">File containing image data. Supports all image formats.</param>
        /// <param name="maxWidth">Maximum width in pixels to enforce. 0 to ignore.</param>
        /// <param name="maxHeight">Maximum height in pixels to engore. 0 to ignore.</param>
        /// <exception cref="ArgumentNullException">Thrown if file is null</exception>
        public BasePrintLogo(BinaryFile file, int maxWidth = 0, int maxHeight = 0)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            // MaxWidth must always be byte aligned (units are in pixels)
            MaxWidth = maxWidth == 0 ? 0 : maxWidth.RoundUp(8);
            MaxHeight = maxHeight;

            SetImageData(file);
        }

        /// <summary>
        /// Create from an existing bitmap which preserves dimensions
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        public BasePrintLogo(Image bitmap)
        {
            MaxWidth = bitmap.Width;
            MaxHeight = bitmap.Height;
            SetImageData(bitmap);
        }

        #region Properties
        /// <summary>
        /// Temporary path for this logo that is used to avoid heap allocating
        /// a buffer just to pass into libcore.
        /// </summary>
        public string TransmitPath { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the raw image data
        /// </summary>
        /// <remarks>Private access, use SetImageData</remarks>
        public Bitmap ImageData { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the dimensions for the current state of the image
        /// </summary>
        public LogoSize Dimensions { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the ideal width of this image. The ideal
        /// width is the scaled width set at instantiation time.
        /// </summary>
        public int IdealWidth { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the ideal height of this image. The ideal
        /// height is the scaled height set at instantiation time.
        /// </summary>
        public int IdealHeight { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the enforced max width. Set to 0 to ignore.
        /// </summary>
        public int MaxHeight { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the enforced max height. Set to 0 to ignore.
        /// </summary>
        public int MaxWidth { get; }

        /// <inheritdoc />
        /// <summary>
        /// Returns true if this image is inverted
        /// </summary>
        public bool IsInverted { get; private set; }
        #endregion

        /// <inheritdoc />
        public void ApplyDithering(DitherAlgorithms ditherAlgorithm, byte threshhold = 128)
        {
            // Create an instance of the specified dithering ditherAlgorithm
            var halftoneProcessor = DitherFactory.GetDitherer(ditherAlgorithm, threshhold);

            var bitmap = ImageData;

            // The big grind
            var dithered = halftoneProcessor.GenerateDithered(bitmap);

            // Update ImageData with dithered result
            SetImageData(dithered);
        }

        /// <inheritdoc />
        /// <summary>
        /// Apply color inversion to this image. Inversion is relative to the source
        /// image. The image begins in the non-inverted state. Calling ApplyColorInversion
        /// once wil put this image in the reverted state. Calling it twice will return it to
        /// the non-inverted state, etc.
        /// </summary>
        public void ApplyColorInversion()
        {
            IsInverted = !IsInverted;
            var bitmap = ImageData;
            bitmap.InvertColorChannels();
            SetImageData(bitmap);
        }

        /// <summary>
        /// Save the current state of this logo as a bitmap at the specified path
        /// </summary>
        /// <param name="outpath">Output path</param>
        public void ExportLogo(string outpath)
        {
            ImageData.Save(outpath);
        }

        /// <summary>
        /// Export the current state of this logo as a binary file at the specific path
        /// </summary>
        /// <param name="outpath">Outpuat path</param>
        public void ExportLogoBin(string outpath)
        {
            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.ToLogoBuffer();

            // Write to file
            File.WriteAllBytes(outpath, bmpData);
        }

        /// <summary>
        /// Export the current state of this logo as a binary file, wrapped in the 1D 76 
        /// ESC/POS bitmap command.
        /// </summary>
        /// <param name="outpath"></param>
        public void ExportLogoEscPos(string outpath)
        {
            // Build up the ESC/POS 1D 76 30 command
            var buffer = new List<byte> {0x1D, 0x76, 0x30, 0x00};

            // Get correct dimensions
            var w = Dimensions.WidthBytes;
            var h = Dimensions.Height;

            // reliance-escpos-commands.readthedocs.io/en/latest/intro.html#
            // Calculate xL and xH
            var xH = (byte)(w / 256);
            var xL = (byte)(w - (xH * 256));

            // Calculate yL and yH
            var yH = (byte)(h / 256);
            var yL = (byte)(h - (yH * 256));

            // Pack up these dimensions
            buffer.Add(xL);
            buffer.Add(xH);
            buffer.Add(yL);
            buffer.Add(yH);

            // Append the bitmap data as a packed dot logo
            var bmpData = ImageData.ToLogoBuffer();
            buffer.AddRange(bmpData);

            // Write to file
            File.WriteAllBytes(outpath, buffer.ToArray());
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns this logo encoded as a bitmap
        /// </summary>
        /// <returns></returns>
        public string AsBase64String()
        {
            using (var bitmap = ImageData)
            {
                return bitmap.ToBase64String();
            }
        }

        /// <summary>
        /// Set the bitmap data from an encoded base64 string
        /// </summary>
        /// <param name="base64">Base64 encoded string</param>
        public void FromBase64String(string base64)
        {
            using (var bitmap = ImageExt.FromBase64String(base64))
            {
                SetImageData(bitmap);
            }
        }

        /// <inheritdoc />
        public byte[] ToBuffer()
        {
            if (ImageData == null)
            {
                return new byte[0];
            }
            using (var bitmap = ImageData)
            {
                return bitmap.ToLogoBuffer();
            }
        }

        /// <summary>
        /// Reads image data into internal image buffer. Scales image down to MaxWidth if required.
        /// Images smaller than MaxWidth will not be scaled up. Result is stored in ImageData field.
        /// Final result CRC is calculated and assigned to CRC32 field.
        /// </summary>
        /// <param name="file">Data to load into image buffer</param>
        private void SetImageData(BinaryFile file)
        {
            if (file.Empty)
            {
                return;
            }

            using(var stream = new MemoryStream(file.GetData()))
            using (var bitmap = Image.FromStream(stream))
            {
                // extract dimensions
                var actualWidth = bitmap.Width;
                var actualHeight = bitmap.Height;


                // Adjust width if needed
                if (MaxWidth != 0 && MaxWidth < actualWidth)
                {
                    IdealWidth = MaxWidth;
                }
                else
                {
                    IdealWidth = actualWidth;
                }


                // Limit height if needed
                if (MaxHeight != 0 && MaxHeight < actualHeight)
                {
                    IdealHeight = MaxHeight;
                }
                else
                {
                    IdealHeight = actualHeight;
                }

                // First, scale width to ideal size
                if (actualWidth > IdealWidth)
                {
                    // Scale down
                    var factor = IdealWidth / (float)actualWidth;
                    actualWidth = (int)(factor * actualWidth);
                    actualHeight = (int)(factor * actualHeight);
                }
                else if (actualWidth < IdealWidth)
                {
                    // Scale up
                    var factor = IdealWidth / (float)actualWidth;
                    actualWidth = (int)(factor * actualWidth);
                    actualHeight = (int)(factor * actualHeight);
                }
                else
                {
                    // Width need not be scaled
                }


                // Second scale height -- down only
                // and don't touch the width, just cut it off
                if (actualHeight > IdealHeight)
                {
                    // Scale down
                    var factor = IdealHeight / (float)actualHeight;
                    actualHeight = (int)(factor * actualHeight);
                }


                // Ensure that whatever width we have is byte aligned
                if (actualWidth % 8 != 0)
                {
                    actualWidth = actualWidth.RoundUp(8);
                }

                // Ensure that our width property matches the final scaled width
                IdealWidth = actualWidth;
                IdealHeight = actualHeight;

                Log.InfoFormat("Logo sized to: {0},{1}", IdealWidth, IdealHeight);

                using (var resized = new Bitmap(bitmap, new Size(IdealWidth, IdealHeight)))
                {
                    SetImageData(resized);
                }
            }
        }

        /// <summary>
        /// Safely copy specified bitmap and use as the source data for this logo
        /// </summary>
        /// <param name="bitmap">Bitmap data to copy and bind to the logo</param>>
        private void SetImageData(Image bitmap)
        {
            // Extract dimension info
            Dimensions = new LogoSize
            {
                Height = bitmap.Height,
                WidthDots = bitmap.Width
            };
            Dimensions.WidthBytes = (int)Math.Ceiling((double)Dimensions.WidthDots / 8);
            Dimensions.SizeInBytes = bitmap.Height * Dimensions.WidthBytes;

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                ImageData = new Bitmap(memory);
            }
        }        
    }
}