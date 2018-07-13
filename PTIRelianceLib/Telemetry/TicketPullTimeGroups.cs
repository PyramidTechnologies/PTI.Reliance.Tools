#region Header
// TicketPullTimeGroups.cs
// PTIRelianceLib
// Cory Todd
// 25-06-2018
// 11:29 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    /// <summary>
    /// Each printed ticket that sits at the bezel starts a clock. The time between
    /// print presented and a customer pulling the ticket is is tracked using binning.
    /// The times are tracked in whole seconds. Only tickets that are pulled are counted
    /// in these groupings. i.e. Ticket retraction and auto-ejected tickets are not counted.
    /// </summary>
    public enum TicketPullTimeGroups
    {
        /// <summary>
        /// Ticket pulled within 4 seconds
        /// </summary>
        Bin4,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin4"/> 4 seconds, up to 8 seconds
        /// </summary>
        Bin8,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin8"/> 8 seconds, up to 16 seconds
        /// </summary>
        Bin16,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin16"/> 16 seconds, up to 32 seconds
        /// </summary>
        Bin32,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin32"/> 32 seconds, up to 50 seconds
        /// </summary>
        Bin50,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin50"/> 50 seconds, up to 70 seconds
        /// </summary>
        Bin70,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin70"/> 70 seconds, up to 90 seconds
        /// </summary>
        Bin90,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin90"/> 90 seconds, up to 120 seconds
        /// </summary>
        Bin120,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin120"/> 120 seconds, up to 180 seconds
        /// </summary>
        Bin180,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin180"/> 180 seconds, up to 300 seconds
        /// </summary>
        Bin300,
        /// <summary>
        /// Ticket pull took more than <see cref="Bin300"/> 300 seconds
        /// </summary>
        BinOvertime
    }
}