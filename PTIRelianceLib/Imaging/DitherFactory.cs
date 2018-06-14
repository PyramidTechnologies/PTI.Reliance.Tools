#region Header

// DitherFactory.cs
// PTIRelianceLib
// Cory Todd
// 13-06-2018
// 1:26 PM

#endregion

namespace PTIRelianceLib.Imaging
{
    /// <summary>
    /// Producer of ditherable data
    /// </summary>
    internal static class DitherFactory
    {
        /// <summary>
        /// Returns the ditherable implementation for the specified ditherAlgorithm
        /// </summary>
        /// <param name="ditherAlgorithm">Algorithm to build</param>
        /// <param name="threshold">Threshold for greyscale</param>
        /// <returns>New ditherale instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
        public static IDitherable GetDitherer(DitherAlgorithms ditherAlgorithm, byte threshold = 128)
        {
            switch (ditherAlgorithm)
            {
                case DitherAlgorithms.Atkinson:
                    // 0,0,1,1,1,1,1,0,0,1,0,0
                    return new Dither(new byte[,]
                    {
                        {0, 0, 1, 1},
                        {1, 1, 1, 0},
                        {0, 1, 0, 0}
                    }, 3, threshold, true);

                case DitherAlgorithms.Burkes:
                    // 0,0,0,8,4,2,4,8,4,2
                    return new Dither(new byte[,]
                    {
                        {0, 0, 0, 8, 4},
                        {2, 4, 8, 4, 2},
                    }, 5, threshold, true);

                case DitherAlgorithms.FloydSteinberg:
                    // 0,0,7,3,5,1
                    return new Dither(new byte[,]
                    {
                        {0, 0, 7},
                        {3, 5, 1},
                    }, 4, threshold, true);

                case DitherAlgorithms.FloydSteinbergFalse:
                    // 0,3,3,2
                    return new Dither(new byte[,]
                    {
                        {0, 3},
                        {3, 2},
                    }, 3, threshold, true);

                case DitherAlgorithms.JarvisJudiceNinke:
                    // 0,0,0,7,5,3,5,7,5,3,1,3,5,3,1
                    return new Dither(new byte[,]
                    {
                        {0, 0, 0, 7, 5},
                        {3, 5, 7, 5, 3},
                        {1, 3, 5, 3, 1}
                    }, 48, threshold);

                case DitherAlgorithms.Sierra:
                    // 0,0,0,5,3,2,4,5,4,2,0,2,3,2,0
                    return new Dither(new byte[,]
                    {
                        {0, 0, 0, 5, 3},
                        {2, 4, 5, 4, 2},
                        {0, 2, 3, 2, 0},
                    }, 5, threshold, true);

                case DitherAlgorithms.Sierra2:
                    // 0,0,0,4,3,1,2,3,2,1
                    return new Dither(new byte[,]
                    {
                        {0, 0, 0, 4, 3},
                        {1, 2, 3, 2, 1},
                    }, 4, threshold, true);

                case DitherAlgorithms.SierraLite:
                    // 0,0,2,1,1,0
                    return new Dither(new byte[,]
                    {
                        {0, 0, 2},
                        {1, 1, 0},
                    }, 2, threshold, true);

                case DitherAlgorithms.Stucki:
                    // 0,0,0,8,4,2,4,8,4,2,1,2,4,2,1
                    return new Dither(new byte[,]
                    {
                        {0, 0, 0, 8, 4},
                        {2, 4, 8, 4, 2},
                        {1, 2, 4, 2, 1},
                    }, 42, threshold);

                default:
                    // We need to at least make it 1bpp bitmap otherwise phoenix will have garbage.
                    return new OneBpp(threshold);
            }
        }
    }
}