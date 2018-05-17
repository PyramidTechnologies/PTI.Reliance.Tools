#region Header
// HidWrapper.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:30 AM
#endregion


using System;

namespace PTIRelianceLib
{
    using System.Diagnostics;
    using PTIRelianceLib.Firmware;
    using PTIRelianceLib.Firmware.Internal;
    using PTIRelianceLib.IO.Internal;
    using PTIRelianceLib.IO;
    using PTIRelianceLib.Protocol;
    using PTIRelianceLib.Transport;

    /// <inheritdoc />
    /// <summary>
    /// Reliance Thermal Printer interface.
    /// </summary>
    [DebuggerDisplay("IsOpen = {_port.IsOpen}")]
    public class ReliancePrinter : IPyramidDevice
    {
        public const int VendorId = 0x0425;
        public const int ProductId = 0x8147;
        private readonly IPort<IPacket> _port;

        /// <summary>
        /// Create a new Reliance Printer. The printer will be discovered automatically.
        /// </summary>
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

        /// <inheritdoc />
        public ReturnCodes SendConfiguration(BinaryFile config)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public BinaryFile ReadConfiguration()
        {
            throw new NotImplementedException();                
        }

        /// <inheritdoc />
        public ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter = null)
        {
            var updater = new RELFwUpdater(_port, firmware)
            {
                Reporter = reporter ?? new DevNullMonitor()
            };
            return updater.ExecuteUpdate();
        }

        /// <inheritdoc />
        public Revlev GetFirmwareRevision()
        {
            var cmd = new ReliancePacket(RelianceCommands.GetRevlev);
            // "Self" param specifies we want revlev for running application
            cmd.Add(0x10);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<Revlev>().Parse(resp);
        }

        /// <inheritdoc />
        public string GetSerialNumber()
        {
            var cmd = new ReliancePacket(RelianceCommands.GetSerialNumber);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<StringModel>().Parse(resp).Data;
        }

        /// <inheritdoc />
        /// <summary>
        /// For printers, all comms are halted while printing (with special exception for ESC/POS realtime status).
        /// If Ping() is called during a print, this will block for a small period of time and then return
        /// unsuccessfully. The resulting ReturnCode will be ExecutionFailure. It is recommended to try this
        /// method multiple times before assuming the printer is offline.
        /// </summary>
        /// <returns>Okay if printer is available, else ExecutionFailure</returns>
        public ReturnCodes Ping()
        {
            var cmd = new ReliancePacket(RelianceCommands.Ping);
            var resp = Write(cmd);
            return resp.GetPacketType() == PacketTypes.PositiveAck ? ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
        }

        /// <inheritdoc />
        /// <summary>
        /// For printers, a reboot (and any power up event) will generate a start up ticket that
        /// calibrates the paper path. Rebooting the printer remotely will cause a paper feed
        /// and leave this ticket at the front bezel. If you have auto-retract enabled, the ticket
        /// will be get pulled back into the kiosk for disposal after a period of time
        /// </summary>
        /// <returns></returns>
        public ReturnCodes Reboot()
        {
            var cmd = new ReliancePacket(RelianceCommands.Reboot);
            var resp = Write(cmd);
            return resp.GetPacketType() == PacketTypes.PositiveAck ?
                ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
        }

        public void Dispose()
        {
            _port?.Dispose();
        }

        /// <summary>
        /// Write wrapper handle the write-wait-read process. The data returned
        /// from this method will be unpackaged for your.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>Response or empty if no response</returns>
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
