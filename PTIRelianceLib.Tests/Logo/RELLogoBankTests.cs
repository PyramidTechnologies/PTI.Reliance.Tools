using PTIRelianceLib.Logo;
using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    using System.Collections.Generic;
    using PTIRelianceLib.Imaging;

    public class RELLogoBankTests : BaseTest
    {
        [Fact]
        public void TestMakeHeader()
        {
            var bitmap = GetResource("white_bitmap.bmp");
            var logoBank = new RELLogoBank();
            var logo = new BasePrintLogo(BinaryFile.From(bitmap));
            var headers = logoBank.MakeHeaders(new List<IPrintLogo>{logo});

            Assert.NotEmpty(headers);

            foreach (var header in headers)
            {
                Assert.NotNull(header);
                Assert.NotNull(header.LogoData);
                Assert.False(string.IsNullOrEmpty(header.Name));
                Assert.Equal(0, header.LeftMargin);
                Assert.Equal(logo.Dimensions.SizeInBytes, (int) header.Size);
                Assert.Equal(logo.Dimensions.Height, header.HeightDots);
                Assert.Equal(logo.Dimensions.WidthBytes, header.WidthBytes);
                Assert.Equal(0x001E0000, (int) header.StartAddr);

                Assert.Equal(131072, logoBank.TotalBankSize);
                Assert.Equal(131072 / 1024, logoBank.TotalBankSizeKb);
            }
        }
    }
}