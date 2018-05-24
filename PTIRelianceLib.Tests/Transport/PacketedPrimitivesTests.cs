using PTIRelianceLib.Transport;
using Xunit;

namespace PTIRelianceLib.Tests.Transport
{

    public class PacketedPrimitivesTests
    {
       [Fact]
        public void TestPacketedBool()
        {
            var packed = new PacketedBool(true);
            Assert.True(packed.Value);
            Assert.Equal(new byte[]{1}, packed.Serialize());
            Assert.Equal(packed, new PacketedBool(true));
            Assert.NotEqual(packed, new PacketedBool(false));

            var parsed = PacketParserFactory.Instance.Create<PacketedBool>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedBool>().Parse(new ReliancePacket());
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedBool>().Parse(new ReliancePacket(1));
            Assert.NotNull(parsed);
            Assert.True(parsed.Value);

            parsed = PacketParserFactory.Instance.Create<PacketedBool>().Parse(new ReliancePacket((byte)0));
            Assert.NotNull(parsed);
            Assert.False(parsed.Value);
        }

        [Fact]
        public void TestPacketedByte()
        {
            var packed = new PacketedByte();
            Assert.Equal(0, packed.Value);
            Assert.Equal(new byte[] { 0 }, packed.Serialize());
            Assert.Equal(packed, new PacketedByte { Value = 0 });
            Assert.NotEqual(packed, new PacketedByte { Value = 1 });

            var parsed = PacketParserFactory.Instance.Create<PacketedByte>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedByte>().Parse(new ReliancePacket());
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedByte>().Parse(new ReliancePacket(1));
            Assert.NotNull(parsed);
            Assert.Equal(1, parsed.Value);

            parsed = PacketParserFactory.Instance.Create<PacketedByte>().Parse(new ReliancePacket((byte)0));
            Assert.NotNull(parsed);
            Assert.Equal(0, parsed.Value);
        }


        [Fact]
        public void TestPacketedShort()
        {
            var packed = new PacketedShort();
            Assert.Equal(0, packed.Value);
            Assert.Equal(new byte[] { 0, 0 }, packed.Serialize());
            Assert.Equal(packed, new PacketedShort { Value = 0 });
            Assert.NotEqual(packed, new PacketedShort { Value = 1 });

            var parsed = PacketParserFactory.Instance.Create<PacketedShort>().Parse(null);
            Assert.Null(parsed);

            // Too short
            parsed = PacketParserFactory.Instance.Create<PacketedShort>().Parse(new ReliancePacket(1));
            Assert.Null(parsed);

            // Too short
            parsed = PacketParserFactory.Instance.Create<PacketedShort>().Parse(new ReliancePacket((byte)0));
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedShort>().Parse(new ReliancePacket(0x10, 0x20));
            Assert.NotNull(parsed);
            Assert.Equal(0x2010, parsed.Value);

            parsed = PacketParserFactory.Instance.Create<PacketedShort>().Parse(new ReliancePacket(0x20, 0x10));
            Assert.NotNull(parsed);
            Assert.Equal(0x1020, parsed.Value);
        }


        [Fact]
        public void TestPacketedInteger()
        {
            var packed = new PacketedInteger();
            Assert.Equal((uint)0, packed.Value);
            Assert.Equal(new byte[] { 0, 0, 0 ,0 }, packed.Serialize());
            Assert.Equal(packed, new PacketedInteger { Value = 0 });
            Assert.NotEqual(packed, new PacketedInteger { Value = 1 });

            var parsed = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(null);
            Assert.Null(parsed);

            // Too short
            parsed = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(new ReliancePacket(1));
            Assert.Null(parsed);

            // Too short
            parsed = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(new ReliancePacket((byte)0));
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(new ReliancePacket(0x10, 0x20, 0x30, 0x40));
            Assert.NotNull(parsed);
            Assert.Equal((uint)0x40302010, parsed.Value);

            parsed = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(new ReliancePacket(0x40, 0x30, 0x20, 0x10));
            Assert.NotNull(parsed);
            Assert.Equal((uint)0x10203040, parsed.Value);
        }

        [Fact]
        public void TestPacketedString()
        {
            var packed = new PacketedString();
            Assert.Equal("", packed.Value);
            Assert.Equal(new byte[] { }, packed.Serialize());
            Assert.Equal(packed, new PacketedString { Value = "" });
            Assert.NotEqual(packed, new PacketedString { Value = "!" });

            var parsed = PacketParserFactory.Instance.Create<PacketedString>().Parse(null);
            Assert.NotNull(parsed);
            Assert.True(string.IsNullOrEmpty(parsed.Value));

            parsed = PacketParserFactory.Instance.Create<PacketedString>().Parse(new ReliancePacket((byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o'));
            Assert.NotNull(parsed);
            Assert.Equal("hello", parsed.Value);

            // Null terminated
            parsed = PacketParserFactory.Instance.Create<PacketedString>().Parse(new ReliancePacket((byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o', 0));
            Assert.NotNull(parsed);
            Assert.Equal("hello", parsed.Value);
        }
    }
}
