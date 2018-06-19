#region Header
// StructuredReader.cs
// PTIRelianceLib
// Cory Todd
// 19-06-2018
// 8:43 AM
#endregion

namespace PTIRelianceLib.IO
{
    using System.Collections.Generic;
    using Protocol;
    using Transport;

    /// <summary>
    /// Reads pages from an IO device and returns result.
    /// </summary>
    internal class StructuredReader
    {
        private readonly IPort<IPacket> _device;

        public StructuredReader(IPort<IPacket> device)
        {
            _device = device;
        }

        public IPacket Read(params byte[] preamble)
        {
            var buffer = new List<byte>();
            var sequenceNum = 0;

            while (true)
            {
                var cmd = _device.Package(preamble);
                var seq = sequenceNum.ToBytesBE();
                cmd.Add(seq);

                _device.Write(cmd);

                var resp = _device.Read(50);
                if (resp.GetPacketType() != PacketTypes.PositiveAck)
                {
                    break;
                }

                buffer.AddRange(resp.GetBytes());
                ++sequenceNum;
            }

            return _device.Package(buffer.ToArray());
        }
    }
}