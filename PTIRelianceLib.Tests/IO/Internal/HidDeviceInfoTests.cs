using System;
using System.Linq;
using Xunit;

namespace PTIRelianceLib.Tests.IO.Internal
{
    using PTIRelianceLib.IO.Internal;

    public class HidDeviceInfoTests
    {
        [Fact]
        public void TestToString()
        {
            var devinfo = new HidDeviceInfo
            {
                VendorId = 0x1234,
                ProductId = 0x5678,
                ManufacturerString = "Company",
                ProductString = "Product",
                Path = "/some/device&1234_5678/4325EACF"
            };

            var str = devinfo.ToString();
            Assert.NotNull(str);

            Assert.Contains(devinfo.ManufacturerString, str);
            Assert.Contains(devinfo.ProductString, str);
            Assert.Contains(devinfo.VendorId.ToString("X4"), str);
            Assert.Contains(devinfo.ProductId.ToString("X4"), str);
        }
    }
}

