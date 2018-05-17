#region Header
// ReliancePacket.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 10:00 AM
#endregion

using System.Diagnostics;
using PTIRelianceLib.Protocol;

namespace PTIRelianceLib.Transport
{
    using System;

    [DebuggerDisplay("Count = {Count}, Type = {_mPacketType}, Content={ToString()}")]
    internal class ReliancePacket : BasePacket
    {
        private PacketTypes _mPacketType = PacketTypes.Unset;

        public ReliancePacket()
        { }

        public ReliancePacket(params byte[] cmd)
            : base(cmd)
        { }

        public ReliancePacket(params RelianceCommands[] cmd)
        {
            foreach (var c in cmd)
            {
                Add((byte)c);
            }
        }

        public override IPacket Package()
        {
            var length = (byte)(Count + 1);
            byte checksum = 0;

            Prepend(length);
            var local = GetBytes();

            for (var i = 0; i < length; i++)
            {
                var nextByte = local[i];
                checksum ^= nextByte;
            }

            Add(checksum);
            IsPackaged = true;
            IsValid = true;
            return this;
        }

        public override IPacket ExtractPayload()
        {
            if (GetPacketType() != PacketTypes.PositiveAck)
            {
                return this;
            }

            // We're not packaged or we're malformed so don't strip away any data
            if (!Validate())
            {
                return new ReliancePacket(GetBytes());
            }

            // There are 2 bytes packaging + the ACK byte so work around that
            var local = GetBytes();
            var totalLen = local[0] - 2;


            ReliancePacket result;

            if (totalLen <= 0)
            {

                // If this is an ACK packet, there is no payload
                // to extract
                result = new ReliancePacket();
            }
            else
            {
                // Otherwise, there is at least 1 data byte to return as payload
                var response = new byte[totalLen];

                // [len ACK D0 D1 ... Dn CSUM]
                Array.Copy(local, 2, response, 0, response.Length);

                result = new ReliancePacket(response);
            }



            result.IsValid = true;
            result.IsPackaged = false;

            // Copy current response type to this child
            result._mPacketType = _mPacketType;

            return result;
        }

        public override int GetExpectedPayloadSize()
        {
            return Count > 0 ? this[0] - 1 : -1;
        }

        protected override bool Validate()
        {
            // If we already know we're malformed, report invalid
            if (_mPacketType == PacketTypes.Malformed)
            {
                return false;
            }

            // Otherwise this is the first validation call so go ahead
            // and perform all packet checks.
            var local = GetBytes();

            // Length sanity check! Shortest possible is 3
            if (Count < 3)
            {
                IsValid = false;
                return false;
            }

            // HID packets should be zero filled so let's omit those
            var totalLen = local[0] + 1;
            if (totalLen < Count)
            {
                var temp = new byte[totalLen];
                Array.Copy(local, temp, temp.Length);
                local = temp;
            }

            // Check for sane length
            var payloadLed = local[0];
            if (payloadLed > local.Length)
            {
                IsValid = false;
                return false;
            }

            var actualChecksum = local[payloadLed];
            byte calculatedChecksum = 0;

            try
            {
                for (var i = 0; i < payloadLed; i++)
                {
                    calculatedChecksum ^= local[i];
                }

                if (actualChecksum != calculatedChecksum)
                {
                    IsValid = false;
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                IsValid = false;
                return false;
            }

            // The packaging is known and valid, set our flag
            IsPackaged = true;
            IsValid = true;
            return true;
        }

        public override PacketTypes GetPacketType()
        {
            // If we've already run this check, report the cached value
            if (_mPacketType != PacketTypes.Unset)
            {
                return _mPacketType;
            }

            // We should always have a 33 bytes packet but
            // be safe and check anyways.
            if (Count == 0)
            {
                _mPacketType = PacketTypes.Timeout;
            }
            else
            {
                // Determine where in the buffer our ACK/NAK byte should be
                var local = GetBytes();
                int ackIndex;

                // If this is true, we've already validated and unpacked
                // this packet.
                if (Count == 1 && IsValid && !IsPackaged)
                {
                    ackIndex = 0;
                }
                else if (Count >= 3)
                {
                    ackIndex = 1;
                }
                else
                {
                    // Otherwise this might be an invalid packet                    
                    _mPacketType = PacketTypes.Malformed;
                    return _mPacketType;
                }

                // What kind of response is this? 
                switch (local[ackIndex])
                {
                    case (byte)ControlCodes.Ack:
                        _mPacketType = PacketTypes.PositiveAck;
                        break;
                    case (byte)ControlCodes.Nak:
                        _mPacketType = PacketTypes.NegativeAck;
                        break;
                    case (byte)ControlCodes.Ser:
                        _mPacketType = PacketTypes.SequenceError;
                        break;
                    case (byte)ControlCodes.Timeout:
                        _mPacketType = PacketTypes.Timeout;
                        break;
                    default:
                        _mPacketType = PacketTypes.Malformed;
                        break;
                }
            }

            // Packet will always contain an ACK message unless it is malformed
            return _mPacketType;
        }
    }
}