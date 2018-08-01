#region Header
// HIDPort.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 9:17 AM
#endregion


namespace PTIRelianceLib.IO
{
    using Logging;
    using Internal;
    using Transport;

    internal class HidPort<T> : IPort<IPacket> where T : IPacket, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private HidWrapper _mHidWrapper;

        public HidPort(HidDeviceConfig config)
        {
            _mHidWrapper = new HidWrapper(config);           
        }

        public IPacket PacketLanguage => new T();

        public IPacket Package(params byte[] data)
        {
            var packet = new T();
            packet.Add(data);
            return packet;
        }

        /// <inheritdoc />
        public string PortPath => _mHidWrapper?.DevicePath;

        public bool IsOpen => _mHidWrapper?.IsOpen == true;

        public bool Open()
        {            
            return _mHidWrapper?.Open() == true;
        }

        public void Close()
        {
            _mHidWrapper?.Close();
        }

        public bool Write(IPacket data)
        {
            if (!data.IsPackaged)
            {
                data.Package();
            }

            var payload = data.GetBytes();
            if (_mHidWrapper?.WriteData(payload) > 0)
            {
                if (Log.IsTraceEnabled())
                {
                    Log.TraceFormat(">> {0}", payload.ByteArrayToHexString());
                }
                return true;
            }

            Log.Error("HID Write failed: {0}", _mHidWrapper?.LastError);

            return false;
        }

        public IPacket Read(int timeoutMs)
        {
            if (_mHidWrapper == null)
            {
                return PacketLanguage;
            }

            var read = _mHidWrapper.ReadData(timeoutMs);
            if (read.Length > 0)
            {
                if (Log.IsTraceEnabled())
                {
                    Log.TraceFormat("<< {0}", read.ByteArrayToHexString());
                }
                return Package(read);
            }


            Log.Error("HID Read failed: {0}", _mHidWrapper.LastError);

            return PacketLanguage;
        }

        public override string ToString()
        {
           return _mHidWrapper == null ? "Disconnected" : _mHidWrapper.ToString();
        }

        public void Dispose()
        {
            _mHidWrapper?.Dispose();
        }
    }
}
