using PTIRelianceLib.Configuration;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    using PTIRelianceLib.Transport;

    public class RELFontTests
    {        
        [Fact]
        public void TestSerialize()
        {
            var font = new RELFont
            {
                FontSize = RelianceFontSizes.A11_B15,
                FontWhich = 'B',
                CodePage = 1252,
            };

            var serialized = font.Serialize();
            var packet = new ReliancePacket(serialized);
            var parsed = PacketParserFactory.Instance.Create<RELFont>().Parse(packet);

            Assert.Equal(font.FontSize, parsed.FontSize);
            Assert.Equal(font.FontWhich, parsed.FontWhich);
            Assert.Equal(font.CodePage, parsed.CodePage);
        }

        [Fact]
        public void TestParseInvalid()
        {
            var parsed = PacketParserFactory.Instance.Create<RELFont>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<RELFont>().Parse(new ReliancePacket(1,2));
            Assert.Null(parsed);
        }

        [Fact]
        public void TestDefaults()
        {
            var font = RELFont.Default;
            Assert.NotNull(font);
            Assert.NotEqual('\0', font.FontWhich);
            Assert.NotEqual(0, font.CodePage);
        }
    }
}
