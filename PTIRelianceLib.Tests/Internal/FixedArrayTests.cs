using System;
using System.Linq;
using Xunit;

namespace PTIRelianceLib.Tests.Internal
{
    public class FixedArrayTests
    {
        [Fact]
        public void TestCtor()
        {
            var farr = new FixedArray<DateTime>(10);
            Assert.Equal(10, farr.Size);
            Assert.Equal(0, farr.Count);
        }

        [Fact]
        public void TestSetData()
        {
            var farr = new FixedArray<BinaryFile>(3);
            
            farr.SetData(
                BinaryFile.From(new byte[0]), 
                BinaryFile.From(new byte[1]),
                BinaryFile.From(new byte[2]), 
                BinaryFile.From(new byte[3]) // This one should be silently discarded
                );
            
            Assert.Equal(farr.Count, farr.Size);

            var data = farr.GetData();
            Assert.Equal(data.Length, farr.Size);

            // Test that each length is what we expect
            var len = 0;
            foreach (var bf in data)
            {
                Assert.Equal(len++, bf.Length);
            }
        }
    }
}
