using Xunit;

namespace PTIRelianceLib.Tests.Telemetry
{
    using PTIRelianceLib.Telemetry;
    using PTIRelianceLib.Transport;

    public class PowerupTelemetryParserTests
    {
        [Fact]
        public void TestIsRegistered()
        {
            PacketParserFactory.Instance.Create<PowerupTelemetry>();
        }

        [Fact]
        public void TestNullPacket()
        {
            // Test the null object is returned
            var parser = PacketParserFactory.Instance.Create<PowerupTelemetry>();
            var tel = parser.Parse(null);
            Assert.Null(tel);
        }

        [Fact]
        public void TestValidLengthPacket()
        {
            // Test that a valid size field can be processed
            var parser = PacketParserFactory.Instance.Create<PowerupTelemetry>();
            var data = new byte[240]; // arbitrary size
            data[4] = 10; // size set to 10

            var tel = parser.Parse(new ReliancePacket(data));
            Assert.NotNull(tel);
        }
    }
}
