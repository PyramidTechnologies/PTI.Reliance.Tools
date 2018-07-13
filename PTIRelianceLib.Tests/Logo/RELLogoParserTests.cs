using Moq;
using PTIRelianceLib.Logo;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    public class RELLogoParserTests
    {
        [Fact]
        public void TestParser()
        {
            var logoParser = new RELLogoParser();

            var result = logoParser.Parse(0, Properties.Resources.white_bitmap);
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