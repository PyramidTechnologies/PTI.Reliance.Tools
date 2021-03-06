﻿#region Header
// ConfigRev.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 9:55 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System.Collections.Generic;
    using Transport;

    /// <summary>
    /// Configuration Revision is embedded in configuration files
    /// to track version number and revision code.
    /// </summary>
    internal class ConfigRev : IParseable
    {
        /// <summary>
        /// Get or Set version, 0 and FF are invalid
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Get or Set revision character, 0 and FF are invalid
        /// </summary>
        public char Revision { get; set; }

        /// <inheritdoc />
        public byte[] Serialize()
        {
            var buff = new List<byte> {Version, (byte) Revision};
            return buff.ToArray();
        }
    }

    internal class ConfigRevParser : BaseModelParser<ConfigRev>
    {
        public override ConfigRev Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null)
            {
                return null;
            }

            if (packet.Count < 2)
            {
                return null;
            }

            return new ConfigRev
            {
                Version = packet[0],
                Revision = (char)packet[1]
            };
        }
    }
}