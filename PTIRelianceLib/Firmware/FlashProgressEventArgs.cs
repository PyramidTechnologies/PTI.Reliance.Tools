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
    /// Flash event used throughout this library
    /// </summary>
    public class FlashProgressEventArgs : System.EventArgs
    {
        /// <summary>
        /// Progress that was reported ranges (0,1.0)
        /// </summary>
        /// <value>Progress value</value>
        public readonly double Progress;

        /// <inheritdoc />
        /// <summary>
        /// Construct new progress event from <paramref name="progress"/>.
        /// </summary>
        /// <param name="progress">Value to report</param>
        public FlashProgressEventArgs(double progress) { Progress = progress; }
    }
}