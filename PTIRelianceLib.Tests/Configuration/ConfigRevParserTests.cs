using PTIRelianceLib.Configuration;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    using PTIRelianceLib.Transport;
    using Transport;

    public class ConfigRevParserTests
    {
        [Fact]
        public void TestValidParse()
        {
            var config = new ConfigRev
            {
                Version = 12,
                Revision = 'D'
            };

            var buff = config.Serialize();

            var packet = new ReliancePacket(buff);
            var parsed = PacketParserFactory.Instance.Create<ConfigRev>().Parse(packet);

            Assert.NotNull(parsed);
            Assert.Equal(config.Version, parsed.Version);
            Assert.Equal(config.Revision, parsed.Revision);
        }

        [Fact]
        public void TestInvalidParse()
        {
            ReliancePacket packet = null;
            var parsed = PacketParserFactory.Instance.Create<ConfigRev>().Parse(packet);
            Assert.Null(parsed);

            packet = new ReliancePacket(1);
            parsed = PacketParserFactory.Instance.Create<ConfigRev>().Parse(packet);
            Assert.Null(parsed);
        }
    }
}
