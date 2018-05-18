﻿#region Header
// ProgressMonitor.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:47 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using Firmware;

    /// <inheritdoc />
    /// <summary>
    /// Event container class for handling all flash update events.
    /// This flashing module will eventually be its own nuget so that is
    /// why it does not implement IProgressReporter.
    /// </summary>
    public class ProgressMonitor : IProgressMonitor
    {
        /// <summary>
        /// Raised when a unit of progress has been made. Called a cummulative
        /// total where units' sum equals 100.
        /// </summary>
        public event EventHandler<FlashProgressEventArgs> OnFlashProgressUpdated;

        /// <summary>
        /// Raised when a message is generated by the controller for the listener
        /// </summary>
        public event EventHandler<FlashEventMessageArgs> OnFlashMessage;


        /// <inheritdoc />
        public virtual void ReportFailure(string format, params object[] args)
        {
            ReportMessage(format, args);
        }

        /// <inheritdoc />
        public virtual void ReportMessage(string format, params object[] args)
        {
            var handler = OnFlashMessage;
            handler?.Invoke(this, new FlashEventMessageArgs(string.Format(format, args)));
        }

        /// <inheritdoc />
        public virtual void ReportProgress(double progress)
        {
            var handler = OnFlashProgressUpdated;
            handler?.Invoke(this, new FlashProgressEventArgs(progress));
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// As the name implies, this consumes and ignores all data 
    /// passed to it.
    /// </summary>
    public class DevNullMonitor : ProgressMonitor
    { }
}