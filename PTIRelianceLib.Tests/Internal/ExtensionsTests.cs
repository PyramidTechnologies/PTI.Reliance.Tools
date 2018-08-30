using Xunit;

namespace PTIRelianceLib.Tests.Internal
{
    using System;
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

        [Fact]
        public void RoundUpIntTests()
        {
            // value, N, expected
            var data = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(0, 5, 5),
                new Tuple<int, int, int>(2, 0, 0),
                new Tuple<int, int, int>(2, 5, 5),
                new Tuple<int, int, int>(20, 8, 24),
                new Tuple<int, int, int>(-16, 8, 16),
            };

            foreach (var t in data)
            {
                Assert.Equal(t.Item3, t.Item1.RoundUp(t.Item2));
            }
        }

        [Fact]
        public void RoundUpLongTests()
        {
            // value, N, expected
            var data = new List<Tuple<long, int, long>>
            {
                new Tuple<long, int, long>(0, 5, 5),
                new Tuple<long, int, long>(2, 0, 0),
                new Tuple<long, int, long>(2, 5, 5),
                new Tuple<long, int, long>(20, 8, 24),
                new Tuple<long, int, long>(-16, 8, 16),
            };

            foreach (var t in data)
            {
                Assert.Equal(t.Item3, t.Item1.RoundUp(t.Item2));
            }
        }

        [Fact]
        public void ToUintBETest()
        {
            // Stores 0x12345678 in big Endian order 
            var input = new byte[] { 0x12, 0x34, 0x56, 0x78 };

            // Reverses 0x12345678 into little Endian order 
            Array.Reverse(input);
            var output = input.ToUintBE();
            Assert.Equal((uint)0x12345678, output);

            // Stores 0x123456 in big Endian order 
            input = new byte[] { 0x12, 0x34, 0x56 };

            // Reverses 0x123456 into little Endian order 
            Array.Reverse(input);
            output = input.ToUintBE();
            Assert.Equal((uint)0x123456, output);

            // Stores 0x1234 in big Endian order 
            input = new byte[] { 0x12, 0x34 };

            // Reverses 0x1234into little Endian order 
            Array.Reverse(input);
            output = input.ToUintBE();
            Assert.Equal((uint)0x1234, output);

            input = new byte[] { 0x12 };
            Array.Reverse(input);
            output = input.ToUintBE();
            Assert.Equal((uint)0x12, output);

            Assert.Throws<ArgumentException>(() => new byte[0].ToUintBE());
        }
    }
}
