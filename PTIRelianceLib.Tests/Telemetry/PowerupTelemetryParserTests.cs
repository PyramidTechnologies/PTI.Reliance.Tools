using Xunit;

namespace PTIRelianceLib.Tests.Telemetry
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
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

        [Fact]
        public void TestSerializeEmpty()
        {
           // Serialize is not implement, should be empty
            var tel = new PowerupTelemetry();
            Assert.Empty(tel.Serialize());
        }

        [Fact]
        public void TestRev3Struct()
        {
            // Test that a valid response payload can be parsed
            var parser = PacketParserFactory.Instance.Create<PowerupTelemetry>();
            var data = Properties.Resources.telemetry_v3;
            var tel = parser.Parse(new ReliancePacket(data));
            Assert.NotNull(tel);

            Assert.Equal(192, tel.AvgCutTime);
            Assert.Equal(252, tel.AvgTimePresented);
            Assert.Equal(0, tel.Button);
            Assert.Equal(0, tel.CriticalErrorCount);
            Assert.Equal(0, tel.CutterCount);
            Assert.Equal(1, tel.HighErrorCount);
            Assert.Equal(0, tel.JammedCount);
            Assert.Equal(0, tel.OverheatedCount);
            Assert.Equal(42688, tel.PaperMovedCount);
            Assert.Equal(1, tel.PaperOut);
            Assert.Equal(0, tel.PaperOutDuringPrint);
            Assert.Equal(0, tel.PlatenOpenCount);
            Assert.Equal(3, tel.StructRevision);
            Assert.Equal(172, tel.StructSize);

            Assert.Equal(9, tel.TicketLengthLog.Size);
            Assert.Equal(9, tel.TicketLengthLog.Count);

            var lens = new[] {28, 1, 0, 0, 0, 0, 1, 0, 1};
            var lenGroups = Enum.GetValues(typeof(TicketLengthGroups)).Cast<TicketLengthGroups>().ToArray();
            for (var i = 0; i < lens.Length; ++i)
            {
                Assert.Equal(lens[i], tel.TicketLengthLog[i]);
                Assert.Equal(lens[i], tel.TicketCountsByLength(lenGroups[i]));
            }            

            var times = new[] {5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            var timeGroups = Enum.GetValues(typeof(TicketPullTimeGroups)).Cast<TicketPullTimeGroups>().ToArray();
            for (var i = 0; i < times.Length; ++i)
            {
                Assert.Equal(times[i], tel.TicketPullTimeLog[i]);
                Assert.Equal(times[i], tel.TicketCountByTimeToPull(timeGroups[i]));
            }

            Assert.Equal(31, tel.TicketsEjected);
            Assert.Equal(5, tel.TicketsPulled);
            Assert.Equal(0, tel.TicketsRetracted);
        }
    }
}