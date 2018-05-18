#region Header
// IDataStreamer.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 9:40 AM
#endregion

namespace PTIRelianceLib.Firmware
{
    using System.Collections.Generic;
    using Transport;

    /// <summary>
    /// This contract describes a class that, given a PacketList and device, can
    /// perform an event-driven flash update.
    /// </summary>
    internal interface IDataStreamer
    {
        /// <summary>
        /// Callback monitor for flash events
        /// </summary>
        IProgressMonitor Reporter { get; set;  }

        /// <summary>
        /// Low-level port for writing and reading responses quickly
        /// </summary>
        IPort<IPacket> Port { get; set; }

        /// <summary>
        /// Transmits packet data to target and handles flow control
        /// </summary>
        /// <param name="packetList">Data to send</param>
        /// <returns>Return code</returns>
        ReturnCodes StreamFlashData(IList<IPacket> packetList);
    }
}