#region Header
// PacketTypes.cs
// PTIRelianceLib
// Cory Todd
// 17-05-2018
// 10:37 AM
#endregion

namespace PTIRelianceLib.Protocol
{
    /// <summary>
    /// Packet types common to protocols. Contextual types
    /// may have different meanings in different protocols. See
    /// the specific packet implementation for details.
    /// </summary>
    internal enum PacketTypes
    {
        /// <summary>
        /// This is not a standard packet type
        /// </summary>
        Normal,

        /// <summary>
        /// A positive acknowledgement or confirmation. AKA ACK
        /// </summary>
        PositiveAck,

        /// <summary>
        /// A negative acknowledgement or confirmation. AKA NAK
        /// </summary>
        NegativeAck,

        /// <summary>
        /// Contextual timeout
        /// </summary>
        Timeout,

        /// <summary>
        /// Contextual sequence error
        /// </summary>
        SequenceError,

        /// <summary>
        /// Phoenix busy signal
        /// </summary>
        Busy,

        /// <summary>
        /// Packet is not following protocol
        /// </summary>
        Malformed,

        /// <summary>
        /// Unknown/Unset packet type
        /// </summary>
        Unset
    }
}