using System;
using System.Linq;
using Xunit;

namespace PTIRelianceLib.Tests.Imaging
{
    using System.Drawing;
    using Properties;
    using PTIRelianceLib.Imaging;

    public class BasePrintLogoTests
    {
        [Fact]
        public void TestCtor()
        {
            var bitmap = BinaryFile.From(Resources.black_bitmap);
            var logo = new BasePrintLogo(bitmap);

            // Dimensions should be unchanged, 48x48
            Assert.Equal(48, logo.IdealHeight);
            Assert.Equal(48, logo.IdealWidth);

            // Max width/height were not specified, should be 0
            Assert.Equal(0, logo.MaxHeight);
            Assert.Equal(0, logo.MaxWidth);

            // Valid data was passed in, ImageData should be valid too
            Assert.NotNull(logo.ImageData);
        }

        [Fact]
        public void TestCtorNullBitmap()
        {
            Assert.Throws<ArgumentNullException>(()=>new BasePrintLogo(null));
        }

        [Fact]
        public void TestApplyDithering()
        {
            var bitmap = BinaryFile.From(Resources.gray_bitmap);
            var logo = new BasePrintLogo(bitmap);

            var startw = logo.IdealWidth;
            var starth = logo.IdealHeight;
            var predither = Crc32.ComputeChecksum(logo.ToBuffer());

            logo.ApplyDithering(DitherAlgorithms.Atkinson);

            // Dimensions should be unchanged, 48x48
            Assert.Equal(starth, logo.IdealHeight);
            Assert.Equal(startw, logo.IdealWidth);

            // Valid data was passed in, ImageData should be valid too
            Assert.NotNull(logo.ImageData);
            var postdither = Crc32.ComputeChecksum(logo.ToBuffer());
            Assert.NotEqual(predither, postdither);
        }
    }
}
