#region Header

// ImageExtTests.cs
// PTIRelianceLib.Tests
// Cory Todd
// 13-06-2018
// 2:08 PM

#endregion

namespace PTIRelianceLib.Tests.Imaging
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using PTIRelianceLib.Imaging;
    using Xunit;

    public class ImageExtTests : BaseTest
    {
        /// <summary>
        /// Given a known bitmap, esnure that it generates the correct colorspace buffer with full opacity
        /// </summary>
        [Fact()]
        public void BitmapToBufferFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var whiteBitmap = GetResource("white_bitmap.bmp");
            var blackBitmap = GetResource("black_bitmap.bmp");
            var redBitmap = GetResource("red_bitmap.bmp");
            var greenBitmap = GetResource("green_bitmap.bmp");
            var blueBitmap = GetResource("blue_bitmap.bmp");

            var inbmp = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expectedBuff =
                ImageTestHelpers.BgraGenerator(new byte[] { 128, 128, 128, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(whiteBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 255, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(blackBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(redBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 255, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(greenBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 255, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(blueBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 0, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));
        }

        [Fact()]
        [Category("BMP")]
        public void BitmapImageToBitmapFact()
        {
            var bmps = new List<Bitmap>
            {
                new BasePrintLogo(BinaryFile.From(GetResource("gray_bitmap.bmp"))).ImageData,
                new BasePrintLogo(BinaryFile.From(GetResource("white_bitmap.bmp"))).ImageData,
                new BasePrintLogo(BinaryFile.From(GetResource("black_bitmap.bmp"))).ImageData,
                new BasePrintLogo(BinaryFile.From(GetResource("red_bitmap.bmp"))).ImageData,
                new BasePrintLogo(BinaryFile.From(GetResource("green_bitmap.bmp"))).ImageData,
                new BasePrintLogo(BinaryFile.From(GetResource("blue_bitmap.bmp"))).ImageData
            };

            foreach (var inbmp in bmps)
            {
                using (var memory = new MemoryStream())
                {
                    inbmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var id = new Bitmap(memory);
                    Assert.True(ImageTestHelpers.CompareCrc32(inbmp, id));
                }
            }
        }


        /// <summary>
        /// Given a known bitmap, esnure that it generates the correct colorspace buffer with full opacity
        /// </summary>
        [Fact()]
        [Category("BMP")]
        public void BitmapInvertColorChannelsFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var whiteBitmap = GetResource("white_bitmap.bmp");
            var blackBitmap = GetResource("black_bitmap.bmp");
            var redBitmap = GetResource("red_bitmap.bmp");
            var greenBitmap = GetResource("green_bitmap.bmp");
            var blueBitmap = GetResource("blue_bitmap.bmp");

            var inbmp = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expectedBuff =
                ImageTestHelpers.BgraGenerator(new byte[] {127, 127, 127, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            var actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(whiteBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 0, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(blackBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(redBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 0, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(greenBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 0, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(blueBitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 255, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);
        }


        [Fact()]
        public void BitmapToLogoBufferSimpleFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var whiteBitmap = GetResource("white_bitmap.bmp");
            var blackBitmap = GetResource("black_bitmap.bmp");
            var redBitmap = GetResource("red_bitmap.bmp");
            var greenBitmap = GetResource("green_bitmap.bmp");
            var blueBitmap = GetResource("blue_bitmap.bmp");

            var inbmp = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            var actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(whiteBitmap)).ImageData;
            expectedBuff = Repeated<byte>(0, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(blackBitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(redBitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(greenBitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(blueBitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);
        }

        [Fact]
        public void TestNullBitmapToBuffer()
        {
            Assert.Empty(ImageExt.ToBuffer(null));
        }

        /// <summary>
        /// Returns a list of type T with value repeat count times
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<T> Repeated<T>(T value, int count)
        {
            var ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }
    }
}