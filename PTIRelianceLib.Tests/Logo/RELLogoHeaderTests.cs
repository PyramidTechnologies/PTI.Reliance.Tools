using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    using PTIRelianceLib.Logo;

    public class RELLogoHeaderTests
    {
        [Fact]
        public void TestSerialize()
        {
            var logoHeader = new RELLogoHeader();
            var data = logoHeader.Serialize();

            Assert.NotNull(data);
            Assert.Equal(28, data.Length);
        }

        [Fact]
        public void TestSerializeShortName()
        {
            var logoHeader = new RELLogoHeader {Name = "A"};
            var data = logoHeader.Serialize();

            Assert.NotNull(data);
            Assert.Equal(28, data.Length);
        }

        [Fact]
        public void TestSerializeLongName()
        {
            var logoHeader = new RELLogoHeader { Name = "AaBbCcDdEeFf" };
            var data = logoHeader.Serialize();

            Assert.NotNull(data);
            Assert.Equal(28, data.Length);
        }
    }
}