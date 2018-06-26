#region Header
// LastTicketState.cs
// PTIRelianceLib
// Cory Todd
// 26-06-2018
// 11:34 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    /// <summary>
    /// Each ticket print is tracked by ejection and length
    /// </summary>
    public struct LastTicketState
    {
        /// <summary>
        /// How was the last ticket handled
        /// </summary>
        public EjectionStatus Status { get; set; }

        /// <summary>
        /// Length of the last ticket in mm
        /// </summary>
        public int LengthMm { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        internal int Reserved { get; set; }
    }
}