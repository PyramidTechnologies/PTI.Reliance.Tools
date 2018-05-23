#region Header
// FlashEventMessageArgs.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:49 AM
#endregion

namespace PTIRelianceLib.Firmware
{
    /// <inheritdoc />
    /// <summary>
    /// Flash event used throughout this library
    /// </summary>
    public class FlashEventMessageArgs : System.EventArgs
    {
        /// <summary>
        /// Flash event message contents.
        /// </summary>
        /// <value>Message contents</value>
        public readonly string Message;

        /// <inheritdoc />
        /// <summary>
        /// Construct new event from <paramref name="message"/>
        /// </summary>
        /// <param name="message">Message to encapsulate</param>
        public FlashEventMessageArgs(string message) { Message = message; }
    }
}