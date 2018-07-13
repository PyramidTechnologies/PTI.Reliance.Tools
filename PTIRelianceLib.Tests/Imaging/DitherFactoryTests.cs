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

    public class DitherFactoryTests
    {
        /// <summary>
        /// Generates various perfect-gray input dithers and compares to known
        /// good dither generators, e.g. photoshop
        /// </summary>
        [Fact()]
        public void GetDithererAtkinsonFact()
        {

            // Input are expected are provided as resources, dithered is what
            // we are testing
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_atkinson)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Atkinson).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererBurkesFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_burkes)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Burkes).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererFloydSteinbergFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_floydsteinbergs)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.FloydSteinberg).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererFloydSteinbergFalseFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_floydsteinbergsfalse)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.FloydSteinbergFalse).GenerateDithered(input);
            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererJarvisJudiceNinkeFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_jjn)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.JarvisJudiceNinke).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        [Category("BMP")]
        public void GetDithererNoneFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.white_bitmap)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.None).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierraFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_sierra)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Sierra).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierra2Fact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_sierra2)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.Sierra2).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererSierraLiteFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_sierralite)).ImageData;
            var dithered = DitherFactory.GetDitherer(DitherAlgorithms.SierraLite).GenerateDithered(input);

            Assert.True(ImageTestHelpers.CompareCrc32(expected, dithered));
        }

        [Fact()]
        public void GetDithererStuckiFact()
        {
            var input = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_bitmap)).ImageData;
            var expected = new BasePrintLogo(BinaryFile.From(Properties.Resources.gray_stucki)).ImageData;
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