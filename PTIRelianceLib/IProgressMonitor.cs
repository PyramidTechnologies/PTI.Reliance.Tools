#region Header
// IProgressReporter.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:01 AM
#endregion

namespace PTIRelianceLib
{
    /// <summary>
    /// The <typeparamref name="IProgressMonitor"/> interface defines the
    /// contract for reporting a task's progress in a variety of formats. It supports
    /// message events and progress reporting as 0-100% value. Workers classes
    /// throughout this library require some form of an <typeparamref name="IProgressMonitor"/>.
    /// Some workers may be multithreaded so it is best practice to delegate these methods
    /// to your UI thread if you intended to use the data on the UI.
    /// </summary>
    public interface IProgressMonitor
    {
        /// <summary>
        /// Called when an iota of progress has been made. For instance, during
        /// flash updating there are a set number of packets to transmit so
        /// each successful packet transmission will call this method.
        /// </summary>
        /// <param name="progress">progress range is (0.0, 1.0)</param>
        void ReportProgress(double progress);

        /// <summary>
        /// Called when a worker class encounters an unrecoverable error. The
        /// details of the error will be packed into a formatted message.
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Args to printf</param>
        void ReportFailure(string format, params object[] args);

        /// <summary>
        /// Called for general worker messages that may indicate specific
        /// points in a process have been reached. Errors and warnings will
        /// not be sent through this method.
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="args">Args to printf</param>
        void ReportMessage(string format, params object[] args);
    }
}