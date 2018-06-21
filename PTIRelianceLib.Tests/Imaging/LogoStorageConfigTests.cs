using Xunit;

namespace PTIRelianceLib.Tests.Imaging
{
    using PTIRelianceLib.Imaging;

    public class LogoStorageConfigTests
    {
        [Fact]
        public void FailIfDefaultsChange()
        {
            var config = LogoStorageConfig.Default;
            Assert.Equal(640, config.MaxPixelWidth);
            Assert.Equal(127, config.Threshold);
            Assert.Equal(DitherAlgorithms.None, config.Algorithm);

            // Make sure properties stay editable
            config.MaxPixelWidth = 0;
            config.Threshold = 0;
            config.Algorithm = DitherAlgorithms.Burkes;
        }

        [Fact]
        public void FailIfDefaultsAreReadonly()
        {
            var config = LogoStorageConfig.Default;

            // Make sure properties stay editable
            config.MaxPixelWidth = 0;
            config.Threshold = 0;
            config.Algorithm = DitherAlgorithms.Burkes;


            Assert.Equal(0, config.MaxPixelWidth);
            Assert.Equal(0, config.Threshold);
            Assert.Equal(DitherAlgorithms.Burkes, config.Algorithm);
        }
    }
}
