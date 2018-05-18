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

    internal class PacketedBoolParser : BaseModelParser<PacketedBool>
    {
        public override PacketedBool Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            return packet == null ? null : new PacketedBool(packet.GetBytes()[0] != 0);
        }
    }

    /// <summary>
    /// Wrapper for byte response codes
    /// </summary>
    public class PacketedByte : IParseable
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

    internal class PacketedByteParser : BaseModelParser<PacketedByte>
    {
        public override PacketedByte Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            return packet == null ? null : new PacketedByte { Value = packet.GetBytes()[0]};
        }
    }

    /// <summary>
    /// Wrapper for short response codes
    /// </summary>
    public class PacketedShort : IParseable
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

    internal class PacketedShortParser : BaseModelParser<PacketedShort>
    {
        public override PacketedShort Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            return packet == null ? null : new PacketedShort { Value = packet.GetBytes().ToUshortBE() };
        }
    }

    /// <summary>
    /// Wrapper for integer response codes
    /// </summary>
    public class PacketedInteger : IParseable
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

    internal class PacketedIntegerParser : BaseModelParser<PacketedInteger>
    {
        public override PacketedInteger Parse(IPacket packet)
        {
            packet = CheckPacket(packet);
            return packet == null ? null : new PacketedInteger { Value = packet.GetBytes().ToUintBE() };
        }
    }
    public class PacketedString : IParseable
    {
        public string Value { get; set; }

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
            return packet == null ? null : new PacketedString { Value = packet.GetBytes().GetPrintableString() };
        }
    }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}