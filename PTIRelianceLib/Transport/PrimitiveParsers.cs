#region Header
// PrimitiveParsers.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 8:21 AM
#endregion


#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
namespace PTIRelianceLib.Transport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using Protocol;

    /// <inheritdoc />
    /// <summary>
    /// Wrapper for integer response codes
    /// </summary>
    internal class PacketedBool : IParseable
    {
        public PacketedBool(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PacketedBool integer &&
                   Value == integer.Value;
        }

        public byte[] Serialize()
        {
            return new [] { (byte) (Value ? 1 : 0) };
        }
    }

    /// <inheritdoc />
    internal class PacketedBoolParser : BaseModelParser<PacketedBool>
    {
        /// <inheritdoc />
        public override PacketedBool Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.IsEmpty)
            {
                return null;
            }
            return new PacketedBool(packet.GetBytes()[0] != 0);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Wrapper for byte response codes
    /// </summary>
    internal class PacketedByte : IParseable
    {
        public byte Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PacketedByte integer &&
                   Value == integer.Value;
        }

        public byte[] Serialize()
        {
            return new[] { Value };
        }
    }

    /// <inheritdoc />
    internal class PacketedByteParser : BaseModelParser<PacketedByte>
    {
        /// <inheritdoc />
        public override PacketedByte Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.IsEmpty)
            {
                return null;
            }
            return new PacketedByte { Value = packet.GetBytes()[0]};
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Wrapper for short response codes
    /// </summary>
    internal class PacketedShort : IParseable
    {
        public ushort Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PacketedShort integer &&
                   Value == integer.Value;
        }

        public byte[] Serialize()
        {            
            return Value.ToBytesBE();
        }
    }

    /// <inheritdoc />
    internal class PacketedShortParser : BaseModelParser<PacketedShort>
    {
        /// <inheritdoc />
        public override PacketedShort Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.Count < 2)
            {
                return null;
            }
            return new PacketedShort { Value = packet.GetBytes().ToUshortBE() };
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Wrapper for integer response codes
    /// </summary>
    internal class PacketedInteger : IParseable
    {
        public uint Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PacketedInteger integer &&
                   Value == integer.Value;
        }

        public byte[] Serialize()
        {
            return Value.ToBytesBE();
        }
    }

    /// <inheritdoc />
    internal class PacketedIntegerParser : BaseModelParser<PacketedInteger>
    {
        /// <inheritdoc />
        public override PacketedInteger Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.Count < 4)
            {
                return null;
            }
            return new PacketedInteger { Value = packet.GetBytes().ToUintBE() };
        }
    }

    /// <inheritdoc />
    internal class PacketedString : IParseable
    {
        public string Value { get; set; } = string.Empty;

        public override bool Equals(object obj)
        {
            return obj is PacketedString str &&
                   Value == str.Value;
        }

        public byte[] Serialize()
        {
            return System.Text.Encoding.ASCII.GetBytes(Value);
        }
    }

    /// <inheritdoc />
    internal class PacketedStringParser : BaseModelParser<PacketedString>
    {
        /// <inheritdoc />
        /// <summary>
        /// Parse response portion of packet as ASCII.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override PacketedString Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            if (packet == null || packet.IsEmpty)
            {
                return new PacketedString {Value = string.Empty };
            }
            return new PacketedString { Value = packet.GetBytes().GetPrintableString() };
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}