namespace PTIRelianceLib.Imaging
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <inheritdoc />
    /// <summary>
    /// One bpp converts the image to a 1 bit per pixel image
    /// </summary>
    internal class OneBpp : Dither
    {
        public OneBpp(byte threshold)
            : base(new byte[,] { { 0 } }, 1, threshold)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Override returns a 1bpp copy of the input bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public override Bitmap GenerateDithered(Bitmap bitmap)
        {
            return new Bitmap(bitmap);
        }
    }
}