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

        private HidDeviceConfig _mConfig;
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
        public void TestDevicePathSet()
        {
            lock (MTestLock)
            {
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    Assert.Equal(FakeNativeMethods.DevicePath, printer.DevicePath);
                }
            }
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
        public void TestStatus()
        {
            lock (MTestLock)
            {
                var data = new Status {HeadVoltage = "23.45"};
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(data.Serialize());
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var resp = printer.GetStatus();                   
                    Assert.Equal(data.HeadVoltage, resp.HeadVoltage);
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
                var count = 0;
                _mNativeMock.GetNextResponse = (d) =>
                {
                    if (count++ != 0)
                    {
                        return GenerateHidData(0xAA);
                    }

                    var rev = new Revlev(1, 27, 127);
                    return GenerateHidData(rev.Serialize());
                };
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var fakeFirmware = new byte[204848];
                    var csum = ((uint)0xCC731E7E).ToBytesBE();
                    var model = ((uint)0x500).ToBytesBE();
                    var size = ((ulong)0x32000).ToBytesBE();
                    Array.Copy(csum, 0, fakeFirmware, 8, csum.Length);
                    Array.Copy(model, 0, fakeFirmware, 12, model.Length);
                    Array.Copy(size, 0, fakeFirmware, 40, size.Length);                    
                    var magic = Encoding.ASCII.GetBytes("PTIXPTIX");
                    Array.Copy(magic, 0, fakeFirmware, 0, magic.Length);
                    var resp = printer.FlashUpdateTarget(BinaryFile.From(fakeFirmware), monitor);
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
            }
        }

        [Fact]
        public void TestFlashUpdateNoReporter()
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

                    var rev = new Revlev(1, 27, 127);
                    return GenerateHidData(rev.Serialize());
                };
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var fakeFirmware = new byte[204848];
                    var csum = ((uint)0xCC731E7E).ToBytesBE();
                    var model = ((uint)0x500).ToBytesBE();
                    var size = ((ulong)0x32000).ToBytesBE();
                    Array.Copy(csum, 0, fakeFirmware, 8, csum.Length);
                    Array.Copy(model, 0, fakeFirmware, 12, model.Length);
                    Array.Copy(size, 0, fakeFirmware, 40, size.Length);
                    var magic = Encoding.ASCII.GetBytes("PTIXPTIX");
                    Array.Copy(magic, 0, fakeFirmware, 0, magic.Length);
                    var resp = printer.FlashUpdateTarget(BinaryFile.From(fakeFirmware));
                    Assert.Equal(ReturnCodes.Okay, resp);
                }
            }
        }

        [Fact]
        public void TestFlashUpdateInvalidFile()
        {
            var sb = new StringBuilder();
            var monitor = new ProgressMonitor();
            monitor.OnFlashMessage += (s, o) => sb.Append(o.Message);
            monitor.OnFlashProgressUpdated += (s, o) => sb.Append(o.Progress);

            lock (MTestLock)
            {
                var count = 0;
                _mNativeMock.GetNextResponse = (d) =>
                {
                    if (count++ != 0)
                    {
                        return GenerateHidData(0xAA);
                    }

                    var rev = new Revlev(1, 27, 127);
                    return GenerateHidData(rev.Serialize());
                };
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    var fakeFirmware = new byte[204848];
                    var magic = Encoding.ASCII.GetBytes("PTIXPTIX");
                    Array.Copy(magic, 0, fakeFirmware, 0, magic.Length);
                    var resp = printer.FlashUpdateTarget(BinaryFile.From(fakeFirmware), monitor);
                    Assert.Equal(ReturnCodes.FlashFileInvalid, resp);
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

        [Fact]
        public void TestWriteRawCmd()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);

                using (var printer = new ProtectedReliancePrinter(_mConfig))
                {
                    var resp = printer._WriteRaw(1);
                    Assert.NotNull(resp);
                    Assert.True(resp.Length == 1);
                }
            }
        }

        [Fact]
        public void TestWriteRawCmdArgs()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(6,7,8,9);

                using (var printer = new ProtectedReliancePrinter(_mConfig))
                {
                    var resp = printer._WriteRaw(1, 2, 3, 4, 5);
                    Assert.NotNull(resp);
                    Assert.True(resp.Length == 4);
                }
            }
        }

        [Fact]
        public void TestWriteRawCmdTooManyArgs()
        {
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);

                using (var printer = new ProtectedReliancePrinter(_mConfig))
                {
                    var args = new byte[33];
                    var resp = printer._WriteRaw(1, args);
                    Assert.NotNull(resp);
                    Assert.True(resp.Length == 0);
                }
            }
        }
      

        [Fact]
        public void TestConnectSpecificPathExists()
        {
            // Test that when we request a specific device path, we get it
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);

                _mConfig.DevicePath = FakeNativeMethods.DevicePath;
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    // If we get a ping, we got a device
                    Assert.Equal(ReturnCodes.Okay, printer.Ping());
                }

                _mConfig.DevicePath = null;
            }
        }

        [Fact]
        public void TestConnectSpecificPathDoesNotExists()
        {
            // Test that when we request a specific device path, we get it
            lock (MTestLock)
            {
                _mNativeMock.GetNextResponse = (d) => GenerateHidData(0xAA);

                _mConfig.DevicePath = "alsonotarealplace";
                using (var printer = new ReliancePrinter(_mConfig))
                {
                    Assert.Equal(ReturnCodes.DeviceNotConnected, printer.Ping());
                }

                _mConfig.DevicePath = null;
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

    /// <summary>
    /// Class for accessing internal state of printer
    /// </summary>
    internal class ProtectedReliancePrinter : ReliancePrinter
    {
        public ProtectedReliancePrinter(HidDeviceConfig config) : base(config)
        {
        }

        /// <summary>
        /// Forward args to internal WriteRaw function
        /// </summary>
        /// <param name="cmd">Byte command</param>
        /// <param name="args">Optional args</param>
        /// <returns>buffer response</returns>
        public byte[] _WriteRaw(byte cmd, params byte[] args)
        {
            return WriteRaw(cmd, args);
        }
    }
}
