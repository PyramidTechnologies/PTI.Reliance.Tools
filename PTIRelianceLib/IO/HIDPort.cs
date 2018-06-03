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

        private readonly HidWrapper _hidWrapper;

        public HidPort(HidDeviceConfig config)
        {
            _hidWrapper = new HidWrapper(config);           
        }

        public IPacket PacketLanguage => new T();

        public IPacket Package(params byte[] data)
        {
            var packet = new T();
            packet.Add(data);
            return packet;
        }

        public bool IsOpen => _hidWrapper.IsOpen;

        public bool Open()
        {
            return _hidWrapper.Open();
        }

        public void Close()
        {
            _hidWrapper.Close();
        }

        public bool Write(IPacket data)
        {
            if (!data.IsPackaged)
            {
                data.Package();
            }

            if (_hidWrapper.WriteData(data.GetBytes()) > 0)
            {
                return true;
            }

            Log.Error("HID Write failed: {0}", _hidWrapper.LastError);
            return false;
        }

        public IPacket Read(int timeoutMs)
        {
            var read = _hidWrapper.ReadData(timeoutMs);
            if (read.Length > 0)
            {
                return Package(read);
            }

            Log.Error("HID Read failed: {0}", _hidWrapper.LastError);
            return PacketLanguage;
        }

        public void Dispose()
        {
            _hidWrapper.Close();
        }

        public override string ToString()
        {
            return _hidWrapper.ToString();
        }
    }
}
