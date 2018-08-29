using PTIRelianceLib.Transport;
using Xunit;

namespace PTIRelianceLib.Tests.Transport
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

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

        [Fact]
        public void TestParseableShorts()
        {
            var packed = new ParseableShortList
            {
                Values = new List<ushort> {0x123, 0x456, 0x789}
            };
            Assert.Equal(3, packed.Values.Count());

            var serialized = packed.Serialize();
            var expected = new byte[] {3, 0x23, 0x01, 0x56, 0x04, 0x89, 0x07};
            Assert.Equal(expected, serialized);
            
            var packet = new ReliancePacket(expected);
            packet.Package();

            var parser= new ParseableShortListParser();
            var unpacked = parser.Parse(packet);
            Assert.Equal(3, unpacked.Values.Count());
            Assert.Equal(packed.Values, unpacked.Values);
        }

        [Fact]
        public void ParseableReturnCodeTest()
        {
            var ack = new ReliancePacket(0xAA);
            ack.Package();

            var parser = PacketParserFactory.Instance.Create<ParseableReturnCode>();
            var parsed = parser.Parse(ack);
            Assert.Equal(ReturnCodes.Okay, parsed.Value);

            var empty = new ReliancePacket();
            Assert.Equal(ReturnCodes.ExecutionFailure, PacketParserFactory.Instance.Create<ParseableReturnCode>().Parse(empty).Value);

            Assert.Equal(new []{(byte)ReturnCodes.Okay}, parsed.Serialize());

            Assert.Equal(ReturnCodes.ExecutionFailure, parser.Parse(null).Value);
        }
    }
}
