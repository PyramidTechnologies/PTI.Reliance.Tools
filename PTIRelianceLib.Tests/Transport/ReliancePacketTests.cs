using PTIRelianceLib.Transport;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Transport
{
    using System.Collections.Generic;
    using Protocol;

    public class ReliancePacketTests
    {       
        [Fact]
        public void TestCreateEmpty()
        {
            var packet = new ReliancePacket();
            packet.Package();
            Assert.True(packet.IsEmpty);
            Assert.False(packet.IsValid);
            Assert.False(packet.IsPackaged);
            Assert.Equal(PacketTypes.Timeout, packet.GetPacketType());
        }

        [Fact]
        public void TestFromBytes()
        {
            var data = new byte[] {1, 2, 3};
            var packet = new ReliancePacket(data);
            Assert.True(data.Length == packet.Count);
            Assert.Equal(data, packet.GetBytes());
            Assert.False(packet.IsValid);
            Assert.False(packet.IsPackaged);
        }

        [Fact]
        public void TestFromCommands()
        {
            var data = new [] { RelianceCommands.GeneralConfigSub, RelianceCommands.AutocutSub };
            var packet = new ReliancePacket(data);
            Assert.True(data.Length == packet.Count);
            Assert.False(packet.IsValid);
            Assert.False(packet.IsPackaged);
        }


        [Fact]
        public void TestAdd()
        {
            var data = new byte[] {1, 2, 3};
            var packet = new ReliancePacket(data);
            packet.Add(4,5,6);
            packet.Prepend(0);

            var expected = new byte[] {0, 1, 2, 3, 4, 5, 6};
            Assert.Equal(expected.Length, packet.Count);
            Assert.Equal(expected, packet.GetBytes());
        }

        [Fact]
        public void TestBadAdd()
        {
            var packet = new ReliancePacket();
            packet.Add(null);
            Assert.True(packet.IsEmpty);

            packet.Add(new ReliancePacket().GetBytes());
            Assert.True(packet.IsEmpty);
        }

        [Fact]
        public void TestInsert()
        {
            var data = new byte[] { 1, 2, 3 };
            var packet = new ReliancePacket(data);
            packet.Add(6, 7);
            packet.Prepend(0);
            packet.Insert(4, 4, 5);

            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            Assert.Equal(expected.Length, packet.Count);
            Assert.Equal(expected, packet.GetBytes());
        }

        [Fact]
        public void TestBadInsert()
        {
            var packet = new ReliancePacket();
            Assert.Throws<IndexOutOfRangeException>(() => packet.Insert(10, new byte[3]));
            Assert.Throws<IndexOutOfRangeException>(() => packet.Insert(-6, new byte[1]));
        }

        [Fact]
        public void TestPackage()
        {
            var packet = new ReliancePacket(RelianceCommands.Ping);
            packet.Package();

            Assert.True(packet.IsValid);
            Assert.True(packet.IsPackaged);
            Assert.Equal(1, packet.GetExpectedPayloadSize());
        }

        [Fact]
        public void TestMalformedExtract()
        {
            var packet = new ReliancePacket(10, 2, 2);
            var extracted = packet.ExtractPayload();

            Assert.False(packet.IsValid);
            Assert.False(packet.IsPackaged);
            Assert.Equal(PacketTypes.Malformed, extracted.GetPacketType());
        }

        [Fact]
        public void TestEgressUnpackage()
        {
            var packet = new ReliancePacket(RelianceCommands.GetFriendlyName);
            packet.Package();

            var payload = packet.ExtractPayload();
            Assert.Equal(1, payload.Count);
            Assert.Equal((byte)RelianceCommands.GetFriendlyName, payload[0]);
        }


        [Fact]
        public void TestIngressUnpackage()
        {
            var payload = new byte[] { 0xAA, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
            var packet = new ReliancePacket(payload);
            packet.Package();

            var extracted = packet.ExtractPayload();

            // ACK byte gets stripped
            Assert.Equal(payload.Length-1, extracted.Count);
            Assert.Equal(PacketTypes.PositiveAck, extracted.GetPacketType());

            Assert.Equal("48, 65, 6C, 6C, 6F", extracted.ToString());
        }

        [Fact()]
        public void TestValidateTooShort()
        {
            var packet = new ReliancePacket(1,2);
            var payload = packet.ExtractPayload();
            Assert.Equal(new byte[]{1,2}, payload.GetBytes());
        }

        [Fact()]
        public void TestValidateNak()
        {
            var codes = new Dictionary<byte, PacketTypes>
            {
                { 0xAC, PacketTypes.NegativeAck},
                { 0xAB, PacketTypes.SequenceError },
                { 0xFF, PacketTypes.Timeout },
                { 0xF1, PacketTypes.Normal },
            };
            foreach (var c in codes)
            {
                var payload = new [] {c.Key, (byte) 'H', (byte) 'e', (byte) 'l', (byte) 'l', (byte) 'o'};

                var packet = new ReliancePacket(payload);
                packet.Package();

                var extracted = packet.ExtractPayload();

                Assert.Equal(payload.Length, extracted.Count);
                Assert.Equal(c.Value, extracted.GetPacketType());

                var str = string.Format("{0:X2}, 48, 65, 6C, 6C, 6F", c.Key);
                Assert.Equal(str, extracted.ToString());
            }
        }

        [Fact]
        public void TestZeroFill()
        {
            var payload = new byte[] { 0xAA, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
            var packet = new MutableReliancePacket(payload);
            packet.Package();

            packet.Mutate(packet.Count, 0, 0, 0, 0, 0, 0, 0 ,0 ,0 ,0);

            var extracted = packet.ExtractPayload();

            // ACK byte gets stripped
            Assert.Equal(payload.Length - 1, extracted.Count);
            Assert.Equal(PacketTypes.PositiveAck, extracted.GetPacketType());

            Assert.Equal("48, 65, 6C, 6C, 6F", extracted.ToString());
        }

        [Fact]
        public void TestBadChecksumIngress()
        {
            var payload = new byte[] { 0xAA, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
            var packet = new MutableReliancePacket(payload);
            packet.Package();

            packet.BreakChecksum();

            var extracted = packet.ExtractPayload();

            // Malformed packet does not get stripped
            Assert.Equal(payload.Length + 2, extracted.Count);
            Assert.Equal(PacketTypes.PositiveAck, extracted.GetPacketType());
        }
    }
}
