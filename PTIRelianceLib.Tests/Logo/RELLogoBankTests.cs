using Moq;
using PTIRelianceLib.Logo;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    using PTIRelianceLib.Imaging;

    public class RELLogoBankTests
    {
        [Fact]
        public void TestMakeHeader()
        {
            var logoBank = new RELLogoBank();
            var logo = new BasePrintLogo(BinaryFile.From(Properties.Resources.white_bitmap));
            var header = logoBank.MakeHeader(logo, 0);

            Assert.NotNull(header);
            Assert.False(string.IsNullOrEmpty(header.Name));
            Assert.Equal(0, header.LeftMargin);
            Assert.Equal(logo.Dimensions.SizeInBytes, (int) header.Size);
            Assert.Equal(logo.Dimensions.Height, header.HeightDots);
            Assert.Equal(logo.Dimensions.WidthBytes, header.WidthBytes);
            Assert.Equal(0x001E0000, (int)header.StartAddr);

            Assert.Equal(131072, logoBank.TotalBankSize);
            Assert.Equal(131072 / 1024, logoBank.TotalBankSizeKb);
        }
    }
}