using PTIRelianceLib.Configuration;
using System;
using Xunit;

namespace PTIRelianceLib.Tests.Configuration
{
    using System.IO;
    using System.Text;

    public class RELConfigTests
    {
        [Fact]
        public void TestInvalidFileLoad()
        {
            var rng = new Random();
            var buff = new byte[1024];
            rng.NextBytes(buff);

            Assert.Throws<PTIException>(()=>RELConfig.Load(BinaryFile.From(buff)));
        }

        [Fact]
        public void TestInvalidFileFieldLoad()
        {
            var config = new RELConfig();
            // Get into a stream
            using (var stream = new MemoryStream())
            {
                config.Save(stream);

                // Corrupt the serial parity field
                var buff = stream.GetBuffer();
                var sb = new StringBuilder();
                for (var i = 0; i <buff.Length; ++i)
                {
                    if (!char.IsLetter((char) buff[i]))
                    {
                        sb.Clear();
                        continue;
                    }

                    sb.Append((char) buff[i]);
                    if (!sb.ToString().Equals("Parity"))
                    {
                        continue;
                    }

                    buff[i + 5] = (byte)'X';
                    break;
                }
              
                Assert.Throws<PTIException>(() => RELConfig.Load(BinaryFile.From(buff)));
            }
        }
    }
}
