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
    using Properties;
    using PTIRelianceLib.Imaging;
    using Xunit;

    public class ImageExtTests
    {
        /// <summary>
        /// Given a known bitmap, esnure that it generates the correct colorspace buffer with full opacity
        /// </summary>
        [Fact()]
        public void BitmapToBufferFact()
        {
            var inbmp = new BasePrintLogo(BinaryFile.From(Resources.gray_bitmap)).ImageData;
            var expectedBuff =
                ImageTestHelpers.BgraGenerator(new byte[] { 128, 128, 128, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.white_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 255, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.black_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.red_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 255, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.green_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 255, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.blue_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 0, 0, 255}, inbmp.Height * inbmp.Width);
            Assert.Equal(ImageConvertResults.Success, ImageTestHelpers.TestBitmapConversion(inbmp, expectedBuff));
        }

        [Fact()]
        [Category("BMP")]
        public void BitmapImageToBitmapFact()
        {
            var bmps = new List<Bitmap>
            {
                new BasePrintLogo(BinaryFile.From(Resources.gray_bitmap)).ImageData,
                new BasePrintLogo(BinaryFile.From(Resources.white_bitmap)).ImageData,
                new BasePrintLogo(BinaryFile.From(Resources.black_bitmap)).ImageData,
                new BasePrintLogo(BinaryFile.From(Resources.red_bitmap)).ImageData,
                new BasePrintLogo(BinaryFile.From(Resources.green_bitmap)).ImageData,
                new BasePrintLogo(BinaryFile.From(Resources.blue_bitmap)).ImageData
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
            var inbmp = new BasePrintLogo(BinaryFile.From(Resources.gray_bitmap)).ImageData;
            var expectedBuff =
                ImageTestHelpers.BgraGenerator(new byte[] {127, 127, 127, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            var actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.white_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 0, 0, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.black_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.red_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 255, 0, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.green_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {255, 0, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.blue_bitmap)).ImageData;
            expectedBuff = ImageTestHelpers.BgraGenerator(new byte[] {0, 255, 255, 255}, inbmp.Height * inbmp.Width);
            inbmp.InvertColorChannels();
            actualBuff = inbmp.ToBuffer();
            Assert.Equal(expectedBuff, actualBuff);
        }


        [Fact()]
        public void BitmapToLogoBufferSimpleFact()
        {
            var inbmp = new BasePrintLogo(BinaryFile.From(Resources.gray_bitmap)).ImageData;
            var expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            var actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.white_bitmap)).ImageData;
            expectedBuff = Repeated<byte>(0, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.black_bitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.red_bitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.green_bitmap)).ImageData;
            expectedBuff = Repeated<byte>(255, (inbmp.Height * inbmp.Width) >> 3).ToArray();
            actualBuff = inbmp.ToLogoBuffer();
            Assert.Equal(expectedBuff, actualBuff);

            inbmp = new BasePrintLogo(BinaryFile.From(Resources.blue_bitmap)).ImageData;
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