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

        [Fact]
        public void TestIndexer()
        {
            var farr = new FixedArray<object>(3);
            var obj1 = new object();
            var obj2 = new object();
            var obj3 = new object();

            farr.SetData(obj1, obj2, obj3);           

            Assert.Equal(obj1, farr[0]);
            Assert.Equal(obj2, farr[1]);
            Assert.Equal(obj3, farr[2]);
        }
    }
}
