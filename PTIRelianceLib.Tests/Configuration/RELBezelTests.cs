using PTIRelianceLib.Configuration;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    using System.Linq;
    using PTIRelianceLib.Transport;
    using Transport;

    public class RELBezelTests
    {
        [Fact]
        public void TestGetDefaults()
        {
            // Should be a 4 item list
            var defaults = RELBezel.GetDefaults();
            Assert.Equal(4, defaults.Count);

            // They must be in this order, per firmware
            Assert.Equal(RELBezelModes.PrinterIdle, defaults[0].BezelMode);
            Assert.Equal(RELBezelModes.TicketPrinting, defaults[1].BezelMode);
            Assert.Equal(RELBezelModes.TicketPresented, defaults[2].BezelMode);
            Assert.Equal(RELBezelModes.TicketEjecting, defaults[3].BezelMode);

            // No duty cycle should be zero
            foreach (var bezel in defaults)
            {
                Assert.NotEqual(0, bezel.DutyCycle);
            }
        }

        [Fact]
        public void TestSerialize()
        {
            var bezel = new RELBezel
            {
                BezelMode = RELBezelModes.PrinterIdle,
                DutyCycle = 100,
                FlashInterval = 0x0100
            };

            var buff = bezel.Serialize();
            var expected = new byte[] {100, 0, 1, 0, 0};
            Assert.Equal(expected, buff);
        }

        [Fact]
        public void TestParse()
        {
            var bezels = RELBezel.GetDefaults();
            Assert.NotEmpty(bezels);
            var config = bezels.First();
            var serialized = config.Serialize();
            var packet = new ReliancePacket(serialized);
            var parsed = PacketParserFactory.Instance.Create<RELBezel>().Parse(packet);
            
            Assert.NotNull(parsed);
            Assert.Equal(config.BezelMode, parsed.BezelMode);
            Assert.Equal(config.DutyCycle, parsed.DutyCycle);
            Assert.Equal(config.FlashInterval, parsed.FlashInterval);
        }

        [Fact]
        public void TestInvalidParse()
        {
            var parsed = PacketParserFactory.Instance.Create<RELBezel>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<RELBezel>().Parse(new ReliancePacket());
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<RELBezel>().Parse(new ReliancePacket(1,2));
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<RELBezel>().Parse(new ReliancePacket(1, 2, 3, 4, 5, 6, 7));
            Assert.Null(parsed);
        }
    }
}
