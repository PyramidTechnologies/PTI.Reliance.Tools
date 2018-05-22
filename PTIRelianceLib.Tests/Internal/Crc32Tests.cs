using System.Collections.Generic;
using Xunit;

namespace PTIRelianceLib.Tests.Internal
{
    public class Crc32Tests
    {

        [Fact]
        public void ComputeChecksumTest()
        {
            var data = new Dictionary<string, uint>
            {

                // Result calculated from https://www.lammertbies.nl/comm/info/crc-calculation.html
                { "", 0 },
                { "a", 0xE8B7BE43 },
                { "abc", 0x352441C2 },
                { "message digest", 0x20159D7F },
                { "abcdefghijklmnopqrstuvwxyz", 0x4C2750BD },
                { "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 0x1FC2E6D2 },
                { "12345678901234567890123456789012345678901234567890123456789012345678901234567890", 0x7CA94A72 },
                { "123456789", 0xCBF43926 }
            };
            foreach (var kv in data)
            {
                var expected = kv.Value;
                var bytes = System.Text.Encoding.ASCII.GetBytes(kv.Key);
                var actual = Crc32.ComputeChecksum(bytes);
                Assert.Equal(expected, actual);
            }
        }
    }
}
