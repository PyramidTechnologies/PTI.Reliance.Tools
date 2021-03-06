﻿#region Header
// IParseable.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 7:18 AM
#endregion

namespace PTIRelianceLib.Transport
{
    /// <summary>
    /// Defines a contract for types that can serialize themselves and be deserialized.
    /// </summary>
    public interface IParseable
    {
        /// <summary>
        /// Writes self to a byte[]
        /// </summary>
        /// <returns>Byte array containing data from this</returns>
        /// <value>Payload data</value>
        byte[] Serialize();
    }
}