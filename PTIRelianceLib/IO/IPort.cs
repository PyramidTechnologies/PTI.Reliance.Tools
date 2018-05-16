#region Header
// IPort.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:51 AM
#endregion

using PTIRelianceLib.Transport;

namespace PTIRelianceLib
{
    internal interface IPort
    {
        /// <summary>
        /// Transmits packet from port
        /// </summary>
        /// <param name="packet">Data to send</param>
        /// <exception cref="PTIException">Raised if data cannot be written</exception>
        void Write(IPacket packet);

        /// <summary>
        /// Reads count bytes from port
        /// </summary>
        /// <param name="count">Count of bytes to read</param>
        /// <returns>Data that was read, if any</returns>
        IPacket Read(int count);
    }
}