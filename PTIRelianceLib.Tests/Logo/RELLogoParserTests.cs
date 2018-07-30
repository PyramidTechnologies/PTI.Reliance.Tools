using Moq;
using PTIRelianceLib.Logo;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    public class RELLogoParserTests : BaseTest
    {
        [Fact]
        public void TestParser()
        {
            var logoParser = new RELLogoParser();
            var bitmap = GetResource("white_bitmap.bmp");
            var result = logoParser.Parse(0, bitmap);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestParserNull()
        {
            var logoParser = new RELLogoParser();

            var result = logoParser.Parse(0, null);
            Assert.Null(result);
        }
    }
}
