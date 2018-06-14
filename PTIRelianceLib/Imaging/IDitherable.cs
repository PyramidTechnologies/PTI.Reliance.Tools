namespace PTIRelianceLib.Imaging
{
    using System.Drawing;

    /// <summary>
    /// A datastructure that can be dithered
    /// </summary>
    internal interface IDitherable
    {
        /// <summary>
        /// Number of rows in algorithm matrix
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Numbers of columns in algorithm matrix
        /// </summary>
        int ColCount { get; }

        /// <summary>
        /// Algorithm's divisor
        /// </summary>
        int Divisor { get; }

        /// <summary>
        /// Black or white threshold limit
        /// </summary>
        byte Threshold { get; }

        /// <summary>
        /// Generates a new, dithered version of the input bitmap using the configured
        /// algorithm parameters.
        /// </summary>
        /// <param name="input">Input bitmap</param>
        /// <returns></returns>
        Bitmap GenerateDithered(Bitmap input);
    }
}