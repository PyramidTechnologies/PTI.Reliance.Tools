using System;
using Xunit;

namespace PTIRelianceLib.Tests
{
    using PTIRelianceLib.Transport;

    public class StatusTests
    {
        [Fact]
        public void TestParser()
        {
            var status = new Status {HeadVoltage = "12.34"};
            Assert.NotNull(status.ToString());

            var serialized = status.Serialize();
            var packet = new ReliancePacket(serialized);
            var parsed = PacketParserFactory.Instance.Create<Status>().Parse(packet);

            Assert.NotNull(parsed);
            Assert.Equal(status.HeadVoltage, parsed.HeadVoltage);
            Assert.Equal(status.HeadTemp, parsed.HeadTemp);

            Assert.Equal(status.SensorStatus, parsed.SensorStatus);
            Assert.Equal(status.TicketStatus, parsed.TicketStatus);
            Assert.Equal(status.PrinterErrors, parsed.PrinterErrors);

            Assert.Equal(status.ArmRaw, parsed.ArmRaw);
            Assert.Equal(status.NotchRaw, parsed.NotchRaw);
            Assert.Equal(status.PathRaw, parsed.PathRaw);
            Assert.Equal(status.PresenterRaw, parsed.PresenterRaw);
            Assert.Equal(status.PaperRaw, parsed.PaperRaw);
        }

        [Fact]
        public void TestInvalidParse()
        {
            var parsed = PacketParserFactory.Instance.Create<Status>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<Status>().Parse(new ReliancePacket());
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<Status>().Parse(new ReliancePacket(4,2,7,6,4));
            Assert.Null(parsed);
        }
    }
}
