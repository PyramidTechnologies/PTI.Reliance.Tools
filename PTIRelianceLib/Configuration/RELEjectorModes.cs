#region Header
// RELEjectorModes.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:22 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Optional eject mode code
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum RelianceEjectorMode
    {
        /// <summary>
        /// Ticket is without from presenting until print completes.
        /// This requires adequate clearance under the printer to hold 
        /// spooling ticket.
        /// </summary>
        [EnumMember(Value = "Presenter Mode")]
        PresenterMode = 0,

        /// <summary>
        /// Ticket is presented as it prints.
        /// </summary>
        [EnumMember(Value = "Continuous Mode")]
        ContinuousMode = 1,

        Invalid = 0xFF

    }
}