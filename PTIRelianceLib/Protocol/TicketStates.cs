#region Header
// TicketStates.cs
// PTIRelianceLib
// Cory Todd
// 22-05-2018
// 7:13 AM
#endregion

namespace PTIRelianceLib.Protocol
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Translates ticket state byte to a state
    /// </summary>
    public enum TicketStates
    {
        /// <summary>
        /// Printer is sitting idle
        /// </summary>
        [EnumMember(Value = "Idle")] Idle,

        /// <summary>
        /// Printer is printing
        /// </summary>
        [EnumMember(Value = "Printing")] Printing,

        /// <summary>
        /// Printer is moving paper towards presenter
        /// </summary>
        [EnumMember(Value = "Un-Presented")] Unpresented,

        /// <summary>
        /// Paper is sitting at presenter
        /// </summary>
        [EnumMember(Value = "Presented")] Presented,

        /// <summary>
        /// We don't know the state of the printer
        /// </summary>
        [EnumMember(Value = "Unknown")] Unknown = 0xFF,
    }
}