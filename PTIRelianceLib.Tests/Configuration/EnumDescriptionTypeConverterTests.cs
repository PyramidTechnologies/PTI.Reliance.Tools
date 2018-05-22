using PTIRelianceLib.Configuration;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    public class EnumDescriptionTypeConverterTests
    {        
        [Fact]
        public void TestValidity()
        {
            var convertor = new EnumDescriptionTypeConverter(typeof(PaperSizes));
            Assert.True(convertor.IsValid(PaperSizes.Roll58Mm));
            // TODO how to test ConvertTo?
        }
    }
}
