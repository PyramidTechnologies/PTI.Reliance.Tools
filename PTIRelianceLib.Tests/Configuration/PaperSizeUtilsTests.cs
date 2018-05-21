using PTIRelianceLib.Configuration;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    public class PaperSizeUtilsTests
    {
        [Fact]
        public void TestToByte()
        {
            Assert.Equal(80, PaperSizeUtils.ToByte(PaperSizes.Roll80Mm));
            Assert.Equal(58, PaperSizeUtils.ToByte(PaperSizes.Roll58Mm));
            Assert.Equal(60, PaperSizeUtils.ToByte(PaperSizes.Roll60Mm));
        }

        [Fact]
        public void TestFromByte()
        {
            // Default is 80mm
            Assert.Equal(PaperSizes.Roll80Mm, PaperSizeUtils.FromByte(100));
            Assert.Equal(PaperSizes.Roll80Mm, PaperSizeUtils.FromByte(80));
            Assert.Equal(PaperSizes.Roll60Mm, PaperSizeUtils.FromByte(60));
            Assert.Equal(PaperSizes.Roll58Mm, PaperSizeUtils.FromByte(58));
        }
    }
}
