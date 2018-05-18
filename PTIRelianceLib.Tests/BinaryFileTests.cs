using Xunit;

namespace PTIRelianceLib.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    public class BinaryFileTests
    {

        [Fact]
        public void BinaryFileTestFromByteArray()
        {
            Assert.True(BinaryFile.From(new byte[0]).Empty);

            var buff = new byte[12];
            Assert.Equal(buff.Length, BinaryFile.From(buff).Length);
            Assert.Equal(buff.LongLength, BinaryFile.From(buff).LongLength);
        }

        [Fact]
        public void BinaryFileTestRandomAccess()
        {
            var buff = Enumerable.Range(0, 100).Select(x => (byte) x).ToArray();
            var file = BinaryFile.From(buff);
            for (var i = 0; i < buff.Length; ++i)
            {
                Assert.Equal(buff[i], file[i]);
            }

            Assert.Equal(buff, file.GetData());
        }

        [Fact]
        public void BinaryFileInvalidBuffer()
        {
            byte[] buff = null;
            Assert.Throws<ArgumentNullException>(() => BinaryFile.From(buff));
        }

        [Fact]
        public void BinaryFileFromPath()
        {

            var path = Path.GetRandomFileName();
            try
            {
                var data = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
                File.WriteAllBytes(path, data);

                var file = BinaryFile.From(path);
                Assert.Equal(data, file.GetData());

            }
            catch (Exception)
            {
                Assert.True(false, "Failed to create test file");
            }
            finally
            {
                File.Delete(path);
            }
        }


        [Fact]
        public void BinaryFileFromNxe()
        {
            var file = BinaryFile.From("illegal_filename_[]@#$");
            Assert.True(file.Empty);
        }
    }
}
