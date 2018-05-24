#region Header
// PTIException.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 8:07 AM
#endregion

namespace PTIRelianceLib
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// The root of all exception classes in PTIRelianceLib. For convenience,
    /// you may wrap library calls with a catcher for this class.
    /// <code>
    /// using(var printer = new ReliancePrinter())
    /// {
    ///     try
    ///     {
    ///         printer.FlashUpdateTarget(myFirmware, myReporter);
    ///     }
    ///     catch(PTIException ex)
    ///     {
    ///         Console.WriteLine(ex.Message);
    ///     }    
    /// }
    /// </code>
    /// </summary>
    public class PTIException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new excception using convenience formatters
        /// </summary>
        /// <param name="fmt">Format string</param>
        /// <param name="args">0 or more args for format string</param>
        public PTIException(string fmt, params object[] args) 
            : base(string.Format(fmt, args))
        {
        }
    }
}