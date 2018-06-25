using System;
using Xunit;

namespace PTIRelianceLib.Tests.Telemetry
{
    using PTIRelianceLib.Telemetry;
    using PTIRelianceLib.Transport;

    public class LifetimeTelemetryParserTests
    {
        [Fact]
        public void TestIsRegistered()
        {
            PacketParserFactory.Instance.Create<LifetimeTelemetry>();
        }

        [Fact]
        public void TestNullPacket()
        {
            // Test the null object is returned
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var tel = parser.Parse(null);
            Assert.Null(tel);
        }

        [Fact]
        public void TestShortPacket()
        {
            // Test the null object is returned
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var tel = parser.Parse(new ReliancePacket(new byte[7]));
            Assert.Null(tel);
        }

        [Fact]
        public void TestMinimumPacket()
        {
            // Test the null object is returned
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var tel = parser.Parse(new ReliancePacket(new byte[8]));
            Assert.Null(tel);
        }

        [Fact]
        public void TestZeroedPacket()
        {
            // Test the null object is returned when struct size field is zero
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var tel = parser.Parse(new ReliancePacket(new byte[112])); // arbitrarily large
            Assert.Null(tel);
        }

        [Fact]
        public void TestMaxedPacket()
        {
            // Test the null object is returned when struct size field is zero
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var data = new byte[112]; // arbitrary size
            Array.Fill<byte>(data, 0xFF); // size set to INT.MAX

            var tel = parser.Parse(new ReliancePacket(data));
            Assert.Null(tel);
        }

        [Fact]
        public void TestInValidLengthPacket()
        {
            // Test that a valid size field can be processed
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var data = new byte[9]; // less than decalred size (10)
            data[4] = 10; // size set to 10

            var tel = parser.Parse(new ReliancePacket(data));
            Assert.Null(tel);
        }

        [Fact]
        public void TestSerializeEmpty()
        {
            // Serialize is not implement, should be empty
            var tel = new LifetimeTelemetry();
            Assert.Empty(tel.Serialize());
        }

        [Fact]
        public void TestValidLengthPacket()
        {
            // Test that a valid size field can be processed
            var parser = PacketParserFactory.Instance.Create<LifetimeTelemetry>();
            var data = new byte[240]; // arbitrary size
            data[4] = 10; // size set to 10

            var tel = parser.Parse(new ReliancePacket(data));
            Assert.NotNull(tel);

            Assert.Equal(0, tel.PowerUpCount);
            Assert.Equal(0, tel.ResetCmdCount);

            tel.PowerUpCount++;
            tel.ResetCmdCount++;

            Assert.Equal(1, tel.PowerUpCount);
            Assert.Equal(1, tel.ResetCmdCount);
        }
    }
}
