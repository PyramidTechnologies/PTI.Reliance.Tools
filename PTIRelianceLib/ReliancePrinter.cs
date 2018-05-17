#region Header
// HidWrapper.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:30 AM
#endregion



namespace PTIRelianceLib
{
    using System.Diagnostics;
    using PTIRelianceLib.Firmware;
    using PTIRelianceLib.Firmware.Internal;
    using PTIRelianceLib.IO.Internal;
    using PTIRelianceLib.IO;
    using PTIRelianceLib.Protocol;
    using PTIRelianceLib.Transport;

    [DebuggerDisplay("IsOpen = {_port.IsOpen}")]
    public class ReliancePrinter : IPyramidDevice
    {
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;
        private readonly IPort<IPacket> _port;

        public ReliancePrinter()
        {
            // Reliance will "always" use report lengths of 34 bytes
            var config = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1
            };
            _port = new HidPort<ReliancePacket>(config);
        }

        public ReturnCodes SendConfiguration(BinaryFile config)
        {
            throw new System.NotImplementedException();
        }

        public ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter = null)
        {
            var updater = new RELFwUpdater(_port, firmware)
            {
                Reporter = reporter ?? new DevNullMonitor()
            };
            return updater.ExecuteUpdate();
        }

        public Revlev GetRevlev()
        {
            var cmd = new ReliancePacket(RelianceCommands.GetRevlev);
            // "Self" param specifies we want revlev for running application
            cmd.Add(0x10);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<Revlev>().Parse(resp);
        }

        public string GetSerialNumber()
        {
            var cmd = new ReliancePacket(RelianceCommands.GetSerialNumber);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<StringModel>().Parse(resp).Data;
        }

        public ReturnCodes Ping()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _port?.Dispose();
        }

        private IPacket Write(IPacket data)
        {
            if (!_port.Write(data))
            {
                return _port.PacketLanguage;
            }

            var resp = _port.Read(10);
            return resp.ExtractPayload();
        }
    }
}
