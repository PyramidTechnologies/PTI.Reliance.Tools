#region Header
// RevlevParserTests.cs
// PTIRelianceLib.Tests
// Cory Todd
// 21-05-2018
// 7:37 AM
#endregion

namespace PTIRelianceLib.Tests
{
    using PTIRelianceLib.Transport;
    using Xunit;

    public class RevlevParserTests
    {
        [Fact]
        public void TestNullPacket()
        {
            ReliancePacket packet = null;
            var parsed = PacketParserFactory.Instance.Create<Revlev>().Parse(packet);
            Assert.NotNull(parsed);
            Assert.Equal("Not Connected", parsed.ToString());
        }

        [Fact]
        public void TestPacketTooSmall()
        {
            for (var i = 0; i < 8; ++i)
            {
                var packet = new ReliancePacket(new byte[i]);
                var parsed = PacketParserFactory.Instance.Create<Revlev>().Parse(packet);
                Assert.NotNull(parsed);
                Assert.Equal("Not Connected", parsed.ToString());
            }
        }

        [Fact]
        public void TestNormalPacket()
        {
            for (var major = 0; major < 8; ++major)
            {
                for (var minor = 0; minor < 8; ++minor)
                {
                    for (var build = 0; build < 8; ++build)
                    {
                        var revlev = new Revlev(major, minor, build);
                        var packet = new ReliancePacket(revlev.Serialize());
                        var parsed = PacketParserFactory.Instance.Create<Revlev>().Parse(packet);
                        Assert.NotNull(parsed);
                        Assert.Equal(revlev, parsed);
                    }
                }
            }
        }
    }
}