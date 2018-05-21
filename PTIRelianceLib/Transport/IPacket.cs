#region Header
// IPyramidDevice.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:10 AM
#endregion

namespace PTIRelianceLib.Transport
{
    using System;
    using Protocol;

    internal interface IPacket
    {
        /// <summary>
        /// Add data to the end of the packet
        /// </summary>
        /// <param name="commands"></param>
        void Add(params byte[] commands);

        /// <summary>
        /// Inserts the data at index
        /// </summary>
        /// <param name="index">Index at which to insert</param>
        /// <param name="data">Data to insert</param>
        /// <exception cref="IndexOutOfRangeException">Raised if index is greater than current length</exception>
        void Insert(int index, params byte[] data);

        /// <summary>
        /// Index overload
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        byte this[int index]
        {
            get;
        }

        /// <summary>
        /// Add all bytes to the beginning of the packet.
        /// </summary>
        /// <param name="bytes"></param>
        void Prepend(params byte[] bytes);

        /// <summary>
        /// Returns true if this is an empty packet
        /// </summary>
        bool IsEmpty { get;}

        /// <summary>
        /// Returns the total length of the packet
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns true if the Package() function has already been called on this instance
        /// </summary>
        bool IsPackaged { get; }

        /// <summary>
        /// Returns true if this packet is known to be valid. This flag is only
        /// set when a packaging or unpackaging operation completes successfully.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Package this data for transmission. This modifies
        /// the current packet and it is also returned, useful for chainging
        /// </summary>
        /// <returns>This packet</returns>
        IPacket Package();

        /// <summary>
        /// Extracts the reponse data from this packet and returns as a new packet.
        /// </summary>
        IPacket ExtractPayload();

        /// <summary>
        /// Returns the current instance as a byte array
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        /// <summary>
        /// Determines what kind of data this packet represents
        /// </summary>
        /// <returns>Packet type</returns>
        PacketTypes GetPacketType();

        /// <summary>
        /// Returns the count in bytes of total
        /// expected payload for this packet. This is
        /// determined by examing the packet header and if
        /// enough is available, a positive number will be 
        /// returned. A negative number means not enough
        /// of the packet header is available to determine.
        /// </summary>
        /// <returns></returns>
        int GetExpectedPayloadSize();

        /// <summary>
        /// Returns this packet as a human-friendly string
        /// </summary>
        /// <returns>string</returns>
        string ToString();
    }
}
