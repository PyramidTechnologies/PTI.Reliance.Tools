#region Header
// FlashProgressEventArgs.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:48 AM
#endregion

namespace PTIRelianceLib.Firmware
{
    /// <inheritdoc />
    /// <summary>
    /// Cross-library progress event
    /// </summary>
    public class FlashProgressEventArgs : System.EventArgs
    {
        /// <summary>
        /// Progress that was reported ranges (0,1.0)
        /// </summary>
        public readonly double Progress;

        /// <inheritdoc />
        /// <summary>
        /// Construct new progress event
        /// </summary>
        /// <param name="progress">Value to report</param>
        public FlashProgressEventArgs(double progress) { Progress = progress; }
    }
}