using Xunit;

namespace PTIRelianceLib.Tests.Internal
{
    using System.Collections.Generic;

    public class ExtensionsTests
    {
        [Fact]
        public void ToUshortBE()
        {
            var data = new Dictionary<byte[], ushort>
            {
                {new byte[] {0, 0}, 0},
                {new byte[] {1, 0}, 1},
                {new byte[] {2, 1}, 258},
                {new byte[] {3, 2}, 515},
                {new byte[] {4, 3}, 772},
                {new byte[] {255, 255}, 0xFFFF}
            };

            foreach (var kv in data)
            {
                var actual1 = kv.Key.ToUshortBE();
                Assert.Equal(kv.Value, actual1);

                // Run the reverse test on the same data
                var actual2 = actual1.ToBytesBE();
                Assert.Equal(kv.Key, actual2);
            }
        }
    }
}
