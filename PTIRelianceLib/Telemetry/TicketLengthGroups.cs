#region Header
// TicketLengthGroups.cs
// PTIRelianceLib
// Cory Todd
// 25-06-2018
// 10:53 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    /// <summary>
    /// Ticket length tracking uses binning to group count of ticket lengths
    /// together. There are 9 length groups that are tracked by length
    /// in millimeters.
    /// </summary>
    public enum TicketLengthGroups
    {
        /// <summary>
        /// Ticket lengths up to 86.375 mm
        /// </summary>
        Bin86 = 0,
        /// <summary>
        /// Ticket lengths from <see cref="Bin86"/>to 118.125 mm
        /// </summary>
        Bin118,
        /// <summary>
        /// Ticket lengths from <see cref="Bin118"/> 118.125 mm to 149.875 mm
        /// </summary>
        Bin149,
        /// <summary>
        /// Ticket lengths from <see cref="Bin149"/> 149.875 mm to 187.875 mm
        /// </summary>
        Bin187,
        /// <summary>
        /// Ticket lengths from <see cref="Bin187"/> 187.875 mm to 226 mm
        /// </summary>
        Bin226,
        /// <summary>
        /// Ticket lengths from <see cref="Bin226"/> 226 mm to 276.875 mm
        /// </summary>
        Bin276,
        /// <summary>
        /// Ticket lengths from <see cref="Bin276"/> 276.875 mm to 327.625 mm
        /// </summary>
        Bin327,
        /// <summary>
        /// Ticket lengths from <see cref="Bin327"/> 327.625 mm to 442 mm
        /// </summary>
        Bin442,
        /// <summary>
        /// Tickets lengths exceeding <see cref="Bin442"/> 442 mm
        /// </summary>
        BinOversized,
    }
}