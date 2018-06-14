using PTIRelianceLib.Flash;
using Xunit;

namespace PTIRelianceLib.Tests.Flash
{
    public class RELStreamerTests
    {

        [Fact]
        public void TestNullStreamer()
        {
            var streamer = new RELStreamer(null, null);
            var status = streamer.StreamFlashData(null);            
            Assert.Equal(ReturnCodes.InvalidRequestPayload, status);
        }
    }
}
