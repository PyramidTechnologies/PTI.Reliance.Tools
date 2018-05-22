using PTIRelianceLib.Configuration;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    using System.Globalization;
    using PTIRelianceLib.Transport;
    using Transport;

    public class RELSerialConfigTests 
    {
        [Fact]
        public void TestDefaultPortConfig()
        {
            var config = new SerialConfigBuilder().Build();
            Assert.NotEqual(0, config.BaudRate);
            Assert.NotEqual(0, config.Databits);
            
            Assert.False(string.IsNullOrEmpty(config.ToString()));

            var buildDefault = SerialConfigBuilder.Default;
            Assert.Equal(config.BaudRate, buildDefault.BaudRate);
            Assert.Equal(config.Databits, buildDefault.Databits);
            Assert.Equal(config.Parity, buildDefault.Parity);
            Assert.Equal(config.Handshake, buildDefault.Handshake);
            Assert.Equal(config.Stopbits, buildDefault.Stopbits);
        }

        [Fact]
        public void TestSerialize()
        {
            var config = new SerialConfigBuilder().Build();
            var serialized = config.Serialize();

            // Should be able to pack, pass into parser, and get back same config
            var packet = new ReliancePacket(serialized);
            var parsed = PacketParserFactory.Instance.Create<RELSerialConfig>().Parse(packet);

            Assert.Equal(config.BaudRate, parsed.BaudRate);
            Assert.Equal(config.Databits, parsed.Databits);
            Assert.Equal(config.Parity, parsed.Parity);
            Assert.Equal(config.Handshake, parsed.Handshake);
            Assert.Equal(config.Stopbits, parsed.Stopbits);
        }

        [Fact]
        public void TestBadParse()
        {
            var parsed = PacketParserFactory.Instance.Create<RELSerialConfig>().Parse(null);
            Assert.Null(parsed);

            var packet = new ReliancePacket(1,3,4);
            parsed = PacketParserFactory.Instance.Create<RELSerialConfig>().Parse(packet);
            Assert.Null(parsed);
        }

        [Fact]
        public void TestBuilder()
        {
            var builder = new SerialConfigBuilder()
                .SetBaudRate(9600)
                .SetDataBits(9)
                .SetFlowControl(SerialHandshake.DtrDsr)
                .SetParity(SerialParity.Even)
                .SetStopBits(SerialStopbits.One);

            var config = builder.Build();
            Assert.Equal(9600, config.BaudRate);
            Assert.Equal(9, config.Databits);
            Assert.Equal(SerialParity.Even, config.Parity);
            Assert.Equal(SerialHandshake.DtrDsr, config.Handshake);
            Assert.Equal(SerialStopbits.One, config.Stopbits);
        }

        [Fact]
        public void TestXonXoff()
        {
            var config = XonXoffConfig.Default();
            Assert.NotEqual(0, config.Xon);
            Assert.NotEqual(0, config.Xoff);

            var serialized = config.Serialize();
            var packet = new ReliancePacket(serialized);
            var parsed = PacketParserFactory.Instance.Create<XonXoffConfig>().Parse(packet);
            Assert.Equal(config.Xon, parsed.Xon);
            Assert.Equal(config.Xoff, parsed.Xoff);

            parsed = PacketParserFactory.Instance.Create<XonXoffConfig>().Parse(null);
            Assert.Null(parsed);

            parsed = PacketParserFactory.Instance.Create<XonXoffConfig>().Parse(new ReliancePacket(1));
            Assert.Null(parsed);
        }
    }
}
