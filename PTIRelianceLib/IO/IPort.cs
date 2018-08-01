#region Header
// IPort.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:51 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using Transport;

    internal interface IPort<T> : IDisposable where T : IPacket
    {
        /// <summary>
        /// Returns an empty packet
        /// </summary>
        T PacketLanguage { get; }

        /// <summary>
        /// Creates a new packet from specified data
        /// </summary>
        /// <param name="data">Data to wrap in a packet</param>
        /// <returns>Packetized data</returns>
        T Package(params byte[] data);

        /// <summary>
        /// Returns the path this device enumerated on
        /// </summary>
        string PortPath { get; }

        /// <summary>
        /// Returns true if port is in an open state
        /// </summary>
        bool IsOpen { get;  }

        /// <summary>
        /// Open port for communication
        /// </summary>
        /// <returns>True if port is opened successfully</returns>
        bool Open();

        /// <summary>
        /// Shutdown, release port
        /// </summary>
        void Close();

        /// <summary>
        /// Transmits packet from port
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>True if write is successfull</returns>
        /// <exception cref="PTIException">Raised if data cannot be written</exception>
        bool Write(T data);

        /// <summary>
        /// Reads data from port and returns result. If timeout is set to -1 this is effectively a blocking read.
        /// When timeout >= 0, if no  data is available before timeout then an empty packet will be returned.
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds for read</param>
        /// <returns>Data that was read, if any</returns>
        /// <exception cref="PTIException">Raised if there is a exception while reading</exception>
        T Read(int timeoutMs);
    }
}