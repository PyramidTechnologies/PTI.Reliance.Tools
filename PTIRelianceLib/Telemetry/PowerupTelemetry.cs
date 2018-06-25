#region Header
// RELTelemetry.cs
// PTIRelianceLib
// Cory Todd
// 19-06-2018
// 7:19 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    using Transport;

    /// <inheritdoc cref="IParseable" />
    /// <summary>
    /// Contains telemtry data describing the usage and lifetime of a Reliance
    /// thermal printer. The data is produced by a Reliance printer and
    /// handled by this API.
    /// </summary>
    public class PowerupTelemetry : IParseable
    {
        internal const int LengthLogLength = 9;
        internal const int PresentLogLength = 11;

        /// <summary>
        /// The revision of this data structure.
        /// </summary>
        internal int StructRevision { get; set; }

        /// <summary>
        /// Size in bytes of this data structure.
        /// </summary>
        internal int StructSize { get; set; }

        /// <summary>
        /// How many cut cycles has there been.
        /// </summary>
        public int CutterCount { get; set; }

        /// <summary>
        /// How many times has the head been opened.
        /// </summary>
        public int PlatenOpenCount { get; set; }

        /// <summary>
        /// How much paper has been moved by the printer
        /// in it's lifetime in units of steps. 1 step = 0.125mm
        /// </summary>
        public int PaperMovedCount { get; set; }

        /// <summary>
        /// How many tickets have been printed. Regardless of size.
        /// </summary>
        public int TicketCount { get; set; }

        /// <summary>
        /// How many tickets have been retracted.
        /// </summary>
        public int TicketsRetracted { get; set; }

        /// <summary>
        /// How many tickets have been ejected.
        /// </summary>
        public int TicketsEjected { get; set; }

        /// <summary>
        /// How many tickets have been pulled by the customer.
        /// </summary>
        public int TicketsPulled { get; set; }

        /// <summary>
        /// Each index is a ticket length range. There are 9 
        /// elements in this array.
        /// </summary>
        internal FixedArray<int> TicketLengthLog { get; set; }

        /// <summary>
        /// Each index is the time a pulled ticket was sitting
        /// at the bezel. Only pulled tickets are logged. There
        /// are 11 elements in this array.
        /// </summary>
        internal FixedArray<int> TicketPullTimeLog { get; set; }

        /// <summary>
        /// Avg time pulled tickets are sitting at the bezel.
        /// </summary>
        public int AvgTimePresented { get; set; }

        /// <summary>
        /// How many times the push button action was used.
        /// </summary>
        public short Button { get; set; }

        /// <summary>
        /// How many times printer ran out of paper during a print job.
        /// </summary>
        public short PaperOutDuringPrint { get; set; }

        /// <summary>
        /// How many times has the printer ran out of paper including during a print job.
        /// </summary>
        public short PaperOut { get; set; }

        /// <summary>
        /// Keeps track of the avg cut time for up to the last 500 cuts.
        /// </summary>
        public short AvgCutTime { get; set; }

        /// <summary>
        /// How many times the printer has jammed.
        /// </summary>
        public short JammedCount { get; set; }

        /// <summary>
        /// How many times the printer has over heated.
        /// </summary>
        public int OverheatedCount { get; set; }

        /// <summary>
        /// How many times the printer was in a critical error.
        /// </summary>
        public int CriticalErrorCount { get; set; }

        /// <summary>
        ///  How many times the printer was in a high priority error.
        /// </summary>
        public short HighErrorCount { get; set; }

        /// <summary>
        /// Returns the count of tickets within the specified ticket 
        /// </summary>
        /// <param name="group">Length group to retrieve</param>
        /// <returns>Number of tickets printed in specified length group</returns>
        public int TicketCountsByLength(TicketLengthGroups group) => TicketLengthLog[(int) group];

        /// <summary>
        /// Returns the count of tickets that pulled by a customer within the specified time bin.
        /// These counts only include tickets pull by the customer. That means that retractions and
        /// ejections are not included in this metric.
        /// </summary>
        /// <param name="group">Time group to retrieve</param>
        /// <returns>Number of tickets in specified time group</returns>
        public int TicketCountByTimeToPull(TicketPullTimeGroups group) => TicketPullTimeLog[(int) group];

        /// <inheritdoc />
        public virtual byte[] Serialize()
        {
            // TODO implement PowerupTelemetry serializer
            return new byte[0];
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Parser class for <see cref="T:PTIRelianceLib.Telemetry.PowerupTelemetry" /> consumes an 
    /// <see cref="T:PTIRelianceLib.Transport.IPacket" />
    /// and produces a <see cref="T:PTIRelianceLib.Telemetry.PowerupTelemetry" />.
    /// </summary>
    internal class PowerupTelemetryParser : BaseModelParser<PowerupTelemetry>
    {
        public override PowerupTelemetry Parse(IPacket packet)
        {
            var parser = new LifetimeTelemetryParser();
            var tel = parser.Parse(packet);
            if(tel == null)
            {
                // Parse failed
                return null;
            }

            // These two fields don't make sense for power up context
            tel.PowerUpCount = 0;
            tel.ResetCmdCount = 0;

            return tel;
        }
    }
}