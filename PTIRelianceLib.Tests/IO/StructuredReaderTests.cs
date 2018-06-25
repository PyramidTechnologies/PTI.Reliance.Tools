using System;
using System.Linq;
using Xunit;

namespace PTIRelianceLib.Tests.IO
{
    using PTIRelianceLib.IO;
    using PTIRelianceLib.IO.Internal;
    using PTIRelianceLib.Transport;

    public class StructuredReaderTests
    {
        private static readonly object MTestLock = new object();

        private readonly HidDeviceConfig _mConfig;
        private readonly FakeNativeMethods _mNativeMock;
        private readonly HidPort<MutableReliancePacket> _mPort;
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;

        public StructuredReaderTests()
        {
            _mNativeMock = new FakeNativeMethods();

            // Reliance will "always" use report lengths of 34 bytes
            _mConfig = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1,
                NativeHid = _mNativeMock
            };

            _mPort = new HidPort<MutableReliancePacket>(_mConfig);
        }

        [Fact]
        public void TestCtor()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                new StructuredReader(_mPort);
            }
        }


        [Fact]
        public void TestReadNak()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAC);
                var reader = new StructuredReader(_mPort);
                var resp = reader.Read(1, 2, 3);
                Assert.True(resp.IsEmpty);
            }
        }

        [Fact]
        public void TestReadAck()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA, 1, 2, 3);
                var reader = new StructuredReader(_mPort);
                var resp = reader.Read(3, 1, 2, 3);
                Assert.False(resp.IsEmpty);
            }
        }

        /// <summary>
        /// Generates a valid inreport
        /// </summary>
        /// <param name="payload">Payload to pack</param>
        /// <returns></returns>
        private byte[] GenerateHidData(params byte[] payload)
        {
            var packet = new ReliancePacket(payload);
            payload = packet.Package().GetBytes();

            var buff = new byte[_mConfig.InReportLength];
            buff[0] = _mConfig.InReportId;
            buff[1] = (byte) payload.Length;
            Array.Copy(payload, 0, buff, 2, Math.Min(payload.Length, buff.Length - 2));
            return buff;
        }
    }
}