#region Header
// DitherFactoryTests.cs
// PTIRelianceLib.Tests
// Cory Todd
// 13-06-2018
// 2:17 PM
#endregion

namespace PTIRelianceLib.Tests.Imaging
{
    using System;
    using System.ComponentModel;
    using PTIRelianceLib.Imaging;
    using Xunit;

    public class DitherFactoryTests : BaseTest
    {
        /// <summary>
        /// Generates various perfect-gray input dithers and compares to known
        /// good dither generators, e.g. photoshop
        /// </summary>
        [Fact()]
        public void GetDithererAtkinsonFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_atkinson.bmp");

            // Input are expected are provided as resources, dithered is what
            // we are testing
            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Atkinson).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererBurkesFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_burkes.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Burkes).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererFloydSteinbergFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_floydsteinbergs.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.FloydSteinberg).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererFloydSteinbergFalseFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_floydsteinbergsfalse.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.FloydSteinbergFalse).GenerateDithered(input);
            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererJarvisJudiceNinkeFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_jjn.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.JarvisJudiceNinke).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        [Category("BMP")]
        public void GetDithererNoneFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("white_bitmap.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.None).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierraFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_sierra.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Sierra).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierra2Fact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_sierra2.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Sierra2).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierraLiteFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_sierralite.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.SierraLite).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererStuckiFact()
        {
            var grayBitmap = GetResource("gray_bitmap.bmp");
            var grayDithered = GetResource("gray_stucki.bmp");

            var input = new BasePrintLogo(BinaryFile.From(grayBitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(grayDithered)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Stucki).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }


        [Fact()]
        public void BadDitherCtorFact()
        {
            // Cannot have a null algorith matrix
            Assert.Throws<ArgumentNullException>(() => new Dither(null, 1, 1));

            // Cannot allow a zero divisor
            Assert.Throws<ArgumentException>(() => new Dither(new byte[,] { }, 0, 0));
        }
    }
}