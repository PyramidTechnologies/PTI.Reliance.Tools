using Xunit;

namespace PTIRelianceLib.Tests.Logo
{
    using System;
    using PTIRelianceLib.IO;
    using PTIRelianceLib.IO.Internal;
    using PTIRelianceLib.Logo;
    using PTIRelianceLib.Transport;

    public class RELLogoUpdaterTests : BaseTest
    {
        private static readonly object MTestLock = new object();

        private readonly HidDeviceConfig _mConfig;
        private readonly FakeNativeMethods _mNativeMock;
        private readonly IProgressMonitor _mReporter;
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;

        public RELLogoUpdaterTests()
        {
            _mReporter = new DevNullMonitor();
            
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
        }

        [Fact]
        public void TestNormalUpdate()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                var port = new HidPort<ReliancePacket>(_mConfig);

                var logo = GetResource("white_bitmap.bmp");
                var header = new RELLogoHeader
                {
                    LogoData = logo,
                };

                var updater = new RELLogoUpdater(port, header)
                {
                    Reporter = _mReporter
                };

                updater.ExecuteUpdate();
            }
        }

        [Fact]
        public void TestEmptyLogo()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                var port = new HidPort<ReliancePacket>(_mConfig);
                var logo = new byte[0];
                var header = new RELLogoHeader
                {
                    LogoData = logo,
                };

                var updater = new RELLogoUpdater(port, header)
                {
                    Reporter = _mReporter
                };

                updater.ExecuteUpdate();
            }
        }

        [Fact]
        public void TestCtorNullPort()
        {
            lock (MTestLock)
            {
                IPort<IPacket> port = null;
                var logo = GetResource("gray_burkes.bmp");
                var header = new RELLogoHeader
                {
                    LogoData = logo,
                };

                Assert.Throws<ArgumentNullException>(() => new RELLogoUpdater(port, header));
            }
        }

        [Fact]
        public void TestCtorNullFile()
        {
            lock (MTestLock)
            {
                var port = new HidPort<ReliancePacket>(_mConfig);
                RELLogoHeader header = null;

                Assert.Throws<ArgumentNullException>(() => new RELLogoUpdater(port, header));
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
            buff[1] = (byte)payload.Length;
            Array.Copy(payload, 0, buff, 2, Math.Min(payload.Length, buff.Length - 2));
            return buff;
        }
    }
}
