#region Header
// MutableReliancePacket.cs
// PTIRelianceLib.Tests
// Cory Todd
// 14-06-2018
// 11:33 AM
#endregion

namespace PTIRelianceLib.Tests
{
    using System;
    using PTIRelianceLib.Transport;

    internal class MutableReliancePacket : ReliancePacket
    {
        internal MutableReliancePacket(byte[] data) : base(data)
        {
        }

        /// <summary>
        /// Injects newVal into byte array at specified index. Does not
        /// modify length or checksum
        /// </summary>
        /// <param name="index">0 base index to insert</param>
        /// <param name="newVal">1 or more bytes to insert</param>
        public void Mutate(int index, params byte[] newVal)
        {
            var data = GetBytes();
            var newData = new byte[data.Length + newVal.Length];
            Array.Copy(data, newData, data.Length);
            Array.Copy(newVal, 0, newData, data.Length, newVal.Length);
            
            Clear();
            Add(newData);
        }

        /// <summary>
        /// Corrupt the checksum on this packet
        /// </summary>
        public void BreakChecksum()
        {
            var data = GetBytes();
            data[Count - 1] = (byte) (data[Count - 1] + 1);
            Clear();
            Add(data);
        }
    }
}