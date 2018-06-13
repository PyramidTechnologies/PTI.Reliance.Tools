#region Header
// PixelTests.cs
// PTIRelianceLib.Tests
// Cory Todd
// 13-06-2018
// 2:04 PM
#endregion

namespace PTIRelianceLib.Tests.Imaging
{
    using PTIRelianceLib.Imaging;
    using Xunit;

    public class PixelTests
    {
        [Fact]
        public void PixelCtorTest()
        {
            var pxDefault = new Pixel();
            Assert.Equal(0, pxDefault.A);
            Assert.Equal(0, pxDefault.R);
            Assert.Equal(0, pxDefault.G);
            Assert.Equal(0, pxDefault.B);

            var pxByte = new Pixel((byte)0, (byte)1, (byte)2, (byte)3);
            Assert.Equal(0, pxByte.A);
            Assert.Equal(1, pxByte.R);
            Assert.Equal(2, pxByte.G);
            Assert.Equal(3, pxByte.B);

            var pxInt = new Pixel(0, 1, 2, 3);
            Assert.Equal(0, pxInt.A);
            Assert.Equal(1, pxInt.R);
            Assert.Equal(2, pxInt.G);
            Assert.Equal(3, pxInt.B);

            var pxBytes = new Pixel(new byte[] { 0, 1, 2, 3 });
            Assert.Equal(3, pxBytes.A);
            Assert.Equal(2, pxBytes.R);
            Assert.Equal(1, pxBytes.G);
            Assert.Equal(0, pxBytes.B);
        }

        [Fact]
        public void PixelWhiteTest()
        {
            var input = new Pixel(new byte[] { 0, 0, 0, 0 });
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 255, 0, 0, 0 }); // Black/Full alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 255, 0, 0 }); // Blue/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 0, 255, 0 }); // Green/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 0, 0, 255 }); // Red/0 alpha is NOT white
            Assert.True(input.IsNotWhite());

            input = new Pixel(new byte[] { 255, 255, 255, 255 }); // White/Full alpha is white
            Assert.False(input.IsNotWhite());

            input = new Pixel(new byte[] { 0, 255, 255, 255 });   // White/0 alpha is white
            Assert.False(input.IsNotWhite());
        }
    }
}