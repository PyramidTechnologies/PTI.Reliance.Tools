#region Header
// BasePacket.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 9:57 AM
#endregion

namespace PTIRelianceLib.Transport
{
    using System;
    using System.Diagnostics;
    using Protocol;

    [DebuggerDisplay("Count = {Count}, Content={ToString()}")]
    internal abstract class BasePacket : IPacket
    {
        private byte[] _mData;

        /// <summary>
        /// Creates an empty packet
        /// </summary>
        protected BasePacket()
        {
            _mData = new byte[0];
            IsPackaged = false;
        }

        /// <summary>
        /// Create a packet with intial data
        /// </summary>
        /// <param name="data"></param>
        protected BasePacket(byte[] data)
        {
            _mData = data;
        }

        #region Methods

        /// <inheritdoc />
        public void Add(params byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            
            // New array will include the old data and new data
            var buffer = new byte[_mData.Length + bytes.Length];

            // Copy in existing data
            Array.Copy(_mData, 0, buffer, 0, _mData.Length);

            // Copy all of new data into buffer starting from the end of the current data
            Array.Copy(bytes, 0, buffer, _mData.Length, bytes.Length);

            _mData = buffer;
            
        }

        /// <inheritdoc />
        public void Insert(int index, params byte[] data)
        {
            // New array will include the old data and new data
            var buffer = new byte[_mData.Length + data.Length];

            // Copy in new data
            Array.Copy(data, 0, buffer, index, data.Length);

            // Copy all of new data into buffer starting from the end of the current data
            Array.Copy(_mData, 0, buffer, index + 1, _mData.Length);

            _mData = buffer;            
        }

        /// <inheritdoc />
        public byte this[int index]
        {
            get => _mData[index];
            set => _mData[index] = value;
        }

        /// <inheritdoc />
        public void Prepend(params byte[] bytes)
        {
            // New array will include the old data and new data
            var buffer = new byte[_mData.Length + bytes.Length];
            Array.Copy(bytes, 0, buffer, 0, bytes.Length);
            Array.Copy(_mData, 0, buffer, bytes.Length, _mData.Length);

            _mData = buffer;

        }

        /// <inheritdoc />
        public bool IsEmpty => _mData.Length == 0;

        /// <inheritdoc />
        public int Count => _mData.Length;

        /// <inheritdoc />
        public bool IsPackaged { get; protected set; }

        /// <inheritdoc />
        public virtual bool IsValid { get; protected set; }

        /// <inheritdoc />
        public virtual int HeaderSize
        {
            get => 2;
            protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        /// <inheritdoc cref="IPacket" />
        public override string ToString()
        {
            return GetBytes().ByteArrayToHexString();
        }

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            return (byte[])_mData.Clone();
        }
        #endregion

        #region Must Implement

        /// <inheritdoc />
        public abstract IPacket Package();

        /// <inheritdoc />
        public abstract IPacket ExtractPayload();

        /// <inheritdoc />
        public abstract int GetExpectedPayloadSize();

        /// <inheritdoc />
        public abstract PacketTypes GetPacketType();
        
        /// <summary>
        /// Validates the current packet contents
        /// </summary>
        /// <returns></returns>
        protected abstract bool Validate();
        #endregion
    }
}