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
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly HidWrapper _mHidWrapper;

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

        public bool IsOpen => _mHidWrapper.IsOpen;

        public bool Open()
        {
            return _mHidWrapper.Open();
        }

        public void Close()
        {
            _mHidWrapper.Close();
        }

        public bool Write(IPacket data)
        {
            if (!data.IsPackaged)
            {
                data.Package();
            }

            if (_mHidWrapper.WriteData(data.GetBytes()) > 0)
            {
                return true;
            }

            Log.Error("HID Write failed: {0}", _mHidWrapper.LastError);
            return false;
        }

        public IPacket Read(int timeoutMs)
        {
            var read = _mHidWrapper.ReadData(timeoutMs);
            if (read.Length > 0)
            {
                return Package(read);
            }

            Log.Error("HID Read failed: {0}", _mHidWrapper.LastError);
            return PacketLanguage;
        }

        public void Dispose()
        {
            _mHidWrapper.Dispose();
        }

        public override string ToString()
        {
            return _mHidWrapper.ToString();
        }
    }
}
