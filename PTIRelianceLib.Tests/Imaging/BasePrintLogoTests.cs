using PTIRelianceLib.Imaging;
using Xunit;

namespace PTIRelianceLib.Tests.Imaging
{
    using System.ComponentModel;

    public class BasePrintLogoTests
    {
        [Fact()]
        [Category("BMP")]
        public void ApplyColorInversionTest()
        {
            // Input are expected are provided as resources, dithered is what
            // we are testing

            var input = BinaryFile.From(Properties.Resources.white_bitmap);

            var logo = new BasePrintLogo(input);

            Assert.False(logo.IsInverted);

            logo.ApplyColorInversion();

            var inverted = logo.ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.black_bitmap)).ImageData;

            // White should ivnert to black
            Assert.True(ImageTestHelpers.CompareCrc32(expected, inverted));
            Assert.True(logo.IsInverted);

            // Flip back to white, test that the inversion flag is cleared
            logo.ApplyColorInversion();
            Assert.False(logo.IsInverted);
        }
    }
}