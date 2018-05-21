using PTIRelianceLib.Transport;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Transport
{
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
        }
    }
}
