#region Header
// EjectionStatus.cs
// PTIRelianceLib
// Cory Todd
// 26-06-2018
// 11:34 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    /// <summary>
    /// A ticket, once printed, terminates in one of two states. On
    /// powerup, the floating state is called NoTicket.
    /// </summary>
    public enum EjectionStatus
    {
        /// <summary>
        /// No ticket state on record
        /// </summary>
        NoTicket = 0,
        /// <summary>
        /// Last ticket was ejected from bezel
        /// </summary>
        Ejected = 1,
        /// <summary>
        /// Last ticket was retracted
        /// </summary>
        Retracted = 2
    }
}