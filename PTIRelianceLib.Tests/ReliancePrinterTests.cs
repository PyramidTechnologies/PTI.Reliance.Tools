using System;
using Xunit;

namespace PTIRelianceLib.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using PTIRelianceLib;
    using PTIRelianceLib.Configuration;
    using PTIRelianceLib.IO.Internal;
    using PTIRelianceLib.Transport;

    public class ReliancePrinterTests
    {
        private static readonly object MTestLock = new object();

        private readonly HidDeviceConfig _mConfig;
        private readonly FakeNativeMethods _mNativeMock;
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;

        public ReliancePrinterTests()
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
        }

        [Fact]
        public void TestPing()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.Ping();
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
            }
        }

        [Fact]
        public void TestReboot()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.Reboot();
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
            }
        }

        [Fact]
        public void TestRevlev()
        {
            lock (MTestLock)
            {
                var rev = new Revlev(1,27,127);
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(rev.Serialize());
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.GetFirmwareRevision();
                    Assert.Equal(rev, resp);
                }
            }
        }

        [Fact]
        public void TestSerialNumber()
        {
            lock (MTestLock)
            {
                const string sn = "123456789";
                var data = Encoding.ASCII.GetBytes(sn);
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(data);
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.GetSerialNumber();
                    Assert.Equal(sn, resp);
                }
            }
        }

        [Fact]
        public void TestGetInstaledCodepages()
        {
            lock (MTestLock)
            {
                var codepages = new ushort[] {1252, 808, 457};
                var data = new List<byte> {0xAA, 3};
                foreach (var cp in codepages)
                {
                    data.AddRange(cp.ToBytesBE());
                }
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(data.ToArray());
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.GetInstalledCodepages();
                    Assert.Equal(codepages, resp);
                }
            }
        }

        [Fact]
        public void TestFlashUpdate()
        {
            var sb = new StringBuilder();
            var monitor = new ProgressMonitor();
            monitor.OnFlashMessage += (s, o) => sb.Append(o.Message);
            monitor.OnFlashProgressUpdated += (s, o) => sb.Append(o.Progress);

            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var fakeFirmware = new byte[204848];
                    var magic = Encoding.ASCII.GetBytes("PTIXPTIX");
                    Array.Copy(magic, 0, fakeFirmware, 0, magic.Length);
                    var resp = printer.FlashUpdateTarget(BinaryFile.From(fakeFirmware), monitor);
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
            }
        }

        [Fact]
        public void TestSetConfig()
        {
            lock (MTestLock)
            {
                var count = 0;

                _mNativeMock.GetNextResponse = (d) =>
                {
                    if (count++ != 0)
                    {
                        return GenerateHidData(0xAA);
                    }

                    var serialConfig = SerialConfigBuilder.Default;
                    return GenerateHidData(serialConfig.Serialize());
                };
                using (var stream = new MemoryStream())
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var config = new RELConfig();
                    config.Save(stream);

                    var binary = BinaryFile.From(stream.GetBuffer());
                    var resp = printer.SendConfiguration(binary);
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
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
            Array.Copy(payload, 0, buff, 2, Math.Min(payload.Length, buff.Length-2));
            return buff;
        }
    }
}
