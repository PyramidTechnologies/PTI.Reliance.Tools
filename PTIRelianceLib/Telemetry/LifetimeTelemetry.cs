#region Header
// LifetimeTelemetry.cs
// PTIRelianceLib
// Cory Todd
// 19-06-2018
// 7:42 AM
#endregion

namespace PTIRelianceLib.Telemetry
{
    using System.IO;
    using Transport;

    /// <inheritdoc />
    /// <summary>
    /// Telemetry data for the lifetime of the printer.
    /// This data persists across flash updates and configuration changes.    
    /// </summary>
    public class LifetimeTelemetry : PowerupTelemetry
    {
        /// <summary>
        /// How many times the printer has powered up.
        /// </summary>
        public int PowerUpCount { get; set; }

        /// <summary>
        /// How many times has the HID reset command been received.
        /// </summary>
        public int ResetCmdCount { get; set; }

        /// <inheritdoc />
        public override byte[] Serialize()
        {
            // TODO implement telemetry serializer
            return new byte[0];
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Parser class for <see cref="T:PTIRelianceLib.Telemetry.LifetimeTelemetry" /> consumes an 
    /// <see cref="T:PTIRelianceLib.Transport.IPacket" />
    /// and produces a <see cref="T:PTIRelianceLib.Telemetry.LifetimeTelemetry" />.
    /// </summary>
    internal class LifetimeTelemetryParser : BaseModelParser<LifetimeTelemetry>
    {
        public override LifetimeTelemetry Parse(IPacket packet)
        {
            // Needs a valid packet and at least 8 bytes to identify type
            packet = CheckPacket(packet);
            if (packet == null || packet.Count < 8)
            {
                return null;
            }
            
            var tel = new LifetimeTelemetry();
            using (var stream = new MemoryStream(packet.GetBytes()))
            using(var reader = new BinaryReader(stream))
            {
                tel.StructRevision = reader.ReadInt32();
                tel.StructSize = reader.ReadInt32();

                // is the size logical?
                if (tel.StructSize == 0 || tel.StructSize == -1)
                {
                    return null;
                }

                // Ensure there is enough data left in the stream to read
                if (packet.Count - 8 <= tel.StructSize)
                {
                    return null;
                }

                tel.PowerUpCount = reader.ReadInt32();
                tel.CutterCount = reader.ReadInt32();
                tel.PlatenOpenCount = reader.ReadInt32();
                tel.PaperMovedCount = reader.ReadInt32();
                tel.TicketCount = reader.ReadInt32();
                tel.TicketsRetracted = reader.ReadInt32();
                tel.TicketsEjected = reader.ReadInt32();
                tel.TicketsPulled = reader.ReadInt32();

                // Read in and parse raw data in appropriate width
                var rawData = reader.ReadBytes(PowerupTelemetry.LengthLogLength * 4);
                using (var blockStream = new MemoryStream(rawData))
                using (var blockReader = new BinaryReader(blockStream))
                {
                    // Read in presented length log, contains 4 byte integers
                    tel.TicketLengthLog = new FixedArray<int>(rawData.Length);
                    for (var i = 0; i < PowerupTelemetry.LengthLogLength; ++i)
                    {
                        tel.TicketLengthLog.SetData(blockReader.ReadInt32());
                    }
                }


                // Read in and parse raw data in appropriate width
                rawData = reader.ReadBytes(PowerupTelemetry.PresentLogLength * 4);
                using (var blockStream = new MemoryStream(rawData))
                using (var blockReader = new BinaryReader(blockStream))
                {               
                    // Read in presented length log, contains 4 byte integers
                    tel.TicketPresentedLog = new FixedArray<int>(rawData.Length);
                    for (var i = 0; i < PowerupTelemetry.PresentLogLength; ++i)
                    {
                        tel.TicketPresentedLog.SetData(blockReader.ReadInt32());
                    }
                }

                tel.AvgTimePresented = reader.ReadInt32();
                tel.Button = reader.ReadInt16();
                tel.PaperOutDuringPrint = reader.ReadInt16();
                tel.PaperOut = reader.ReadInt16();
                tel.AvgCutTime = reader.ReadInt16();
                tel.ResetCmdCount = reader.ReadInt32();
                tel.JammedCount = reader.ReadInt16();
                tel.OverheatedCount = reader.ReadInt32();
                tel.CriticalErrorCount = reader.ReadInt32();
                tel.HighErrorCount = reader.ReadInt16();
            }

            return tel;
        }
    }
}