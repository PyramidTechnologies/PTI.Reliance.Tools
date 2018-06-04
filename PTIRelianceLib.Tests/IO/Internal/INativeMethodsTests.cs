namespace PTIRelianceLib.Tests.IO.Internal
{
    using System;
    using Moq;
    using PTIRelianceLib.IO.Internal;
    using Xunit;

    public class INativeMethodsTests
    {
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;

        [Fact]
        public void TestCloseIsCalledOnScopeClosed()
        {

            var mockedNativeMethods = new Mock<INativeMethods>();
            var config = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1,
                NativeHid = mockedNativeMethods.Object
            };

            using (var printer = new ReliancePrinter(config))
            {
                // Call any method so this is not optimized away
                printer.Ping();
            }

            // Don't care what device was passd in as long as Close() is called
            mockedNativeMethods.Verify(x => x.Close(It.IsAny<HidDevice>()), Times.Once);
        }

        [Fact]
        public void TestCloseIsCalledOnReboot()
        {

            var mockedNativeMethods = new Mock<INativeMethods>();
            var config = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1,
                NativeHid = mockedNativeMethods.Object
            };

            using (var printer = new ReliancePrinter(config))
            {
                printer.Reboot();
            }            
            mockedNativeMethods.Verify(x => x.Close(It.IsAny<HidDevice>()), Times.Once);

            using (var printer = new ReliancePrinter(config))
            {
                printer.EnterBootloader();
            }
            mockedNativeMethods.Verify(x => x.Close(It.IsAny<HidDevice>()), Times.Exactly(2));
        }
    }
}
