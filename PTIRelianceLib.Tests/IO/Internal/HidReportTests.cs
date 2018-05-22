using PTIRelianceLib.IO;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.IO.Internal
{
    using PTIRelianceLib.IO.Internal;

    public class HidReportTests
    {
        [Fact]
        public void TestRELInputReport()
        {
            // Reliance will "always" use report lengths of 34 bytes
            var config = new HidDeviceConfig
            {
                VendorId = 0x1234,
                ProductId = 0x5678,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1
            };

            var inreport = HidReport.MakeInputReport(config);
            Assert.Equal(config.InReportLength, inreport.Data.Length);
            Assert.Equal(config.InReportId, inreport.Data[0]);
            Assert.Equal(config.InReportId, inreport.ReportId);
        }

        [Fact]
        public void TestRELOutputReport()
        {
            // Reliance will "always" use report lengths of 34 bytes
            var config = new HidDeviceConfig
            {
                VendorId = 0x1234,
                ProductId = 0x5678,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1
            };

            var outreport = HidReport.MakeOutputReport(config, new byte[]{1});
            Assert.Equal(config.OutReportLength, outreport.Data.Length);
            Assert.Equal(config.OutReportId, outreport.Data[0]);
            Assert.Equal(config.OutReportId, outreport.ReportId);

            var payload = outreport.GetPayload();
            Assert.Single(payload);

            // Catch if someone change Size definition
            var expected = new UIntPtr(Convert.ToUInt32(outreport.Data.Length));
            Assert.Equal(expected, outreport.Size);
        }


        [Fact]
        public void TestRELEmptyOutputReport()
        {
            // Reliance will "always" use report lengths of 34 bytes
            var config = new HidDeviceConfig
            {
                VendorId = 0x1234,
                ProductId = 0x5678,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1
            };

            var outreport = HidReport.MakeOutputReport(config, new byte[0]);
            Assert.Equal(config.OutReportLength, outreport.Data.Length);
            Assert.Equal(config.OutReportId, outreport.Data[0]);
            Assert.Equal(config.OutReportId, outreport.ReportId);
        }
    }
}
