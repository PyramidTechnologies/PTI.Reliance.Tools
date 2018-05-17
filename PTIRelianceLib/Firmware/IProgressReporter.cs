#region Header
// IProgressReporter.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:01 AM
#endregion

namespace PTIRelianceLib.Firmware
{
    /// <summary>
    /// Interface for reporting a task's progress in a variety of formats.
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>
        /// Receive a progress report
        /// </summary>
        /// <param name="progress">progress range is (0.0, 1.0)</param>
        void ReportProgress(double progress);

        /// <summary>
        /// Reports a failure that halts action progress
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Args to printf</param>
        void ReportFailure(string format, params object[] args);

        /// <summary>
        /// Reports a message that does NOT halt action progress
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Args to printf</param>
        void ReportMessage(string format, params object[] args);
    }
}