#region Header
// HIDPort.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 9:17 AM
#endregion


using PTIRelianceLib.Transport;

namespace PTIRelianceLib.IO
{
    using System.Diagnostics;
    using PTIRelianceLib.IO.Internal;

    internal class HidPort<T> : IPort<IPacket> where T : IPacket, new()
    {
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

            Trace.WriteLine(string.Format("HID Write failed: {0}", _hidWrapper.LastError));
            return false;
        }

        public IPacket Read(int timeoutMs)
        {
            var read = _hidWrapper.ReadData(timeoutMs);
            if (read.Length > 0)
            {
                return Package(read);
            }

            Trace.WriteLine(string.Format("HID Read failed: {0}", _hidWrapper.LastError));
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
