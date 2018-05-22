#region Header
// RELTicketAction.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:25 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Reliance control codes for actions related to removing ticket from paper path
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum NewTicketAction
    {
        /// <summary>
        /// No option
        /// </summary>
        [EnumMember(Value = "Invalid Option")]
        Invalid = 0,
        /// <summary>
        /// Eject ticket forward
        /// </summary>
        [EnumMember(Value = "Eject Ticket")]
        EjectTicket = 1,
        /// <summary>
        /// Retract ticket backward
        /// </summary>
        [EnumMember(Value = "Retract Ticket")]
        RetractTicket = 2,
    }

    /// <summary>
    /// Reliance control codes for actions related to removing ticket from paper path
    /// after a period of time
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum TicketTimeoutAction
    {
        /// <summary>
        /// Do nothing - leave paper until next print event
        /// </summary>
        [EnumMember(Value = "Nothing")]
        Nothing = 0,
        /// <summary>
        /// Eject forward after period of time
        /// </summary>
        [EnumMember(Value = "Eject Ticket")]
        EjectTicket = 1,
        /// <summary>
        /// Retract backward after period of time
        /// </summary>
        [EnumMember(Value = "Retract Ticket")]
        RetractTicket = 2,
        /// <summary>
        /// Unconfigured
        /// </summary>
        [EnumMember(Value = "Invalid Option")]
        Invalid = 0xFF,
    }
}