#region Header
// HidWrapper.cs
// PTIRelianceLib
// Cory Todd
// 16-05-2018
// 11:30 AM
#endregion

namespace PTIRelianceLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Configuration;
    using Firmware.Internal;
    using IO;
    using IO.Internal;
    using Protocol;
    using Transport;

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
        /// <exception cref="PTIException">Raised if native HID library cannot be loaded</exception> 
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
                OutReportId = 1,
                NativeHid = new NativeMethods()
            };

            try
            {
                _port = new HidPort<ReliancePacket>(config);
            }
            catch (DllNotFoundException ex)
            {
                // Re throw as our own exception
                throw new PTIException("Failed to load HID library: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Internal constructor using specified configuration
        /// </summary>
        /// <param name="config">HID library options</param>
        internal ReliancePrinter(HidDeviceConfig config)
        {
            try
            {
                _port = new HidPort<ReliancePacket>(config);
            }
            catch (DllNotFoundException ex)
            {
                // Re throw as our own exception
                throw new PTIException("Failed to load HID library: {0}", ex.Message);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Parses configuraion and send to target printer.
        /// </summary>
        /// <param name="config">Configuration to send</param>
        /// <returns>Okay on success, otherwise error code</returns>
        /// <exception cref="T:PTIRelianceLib.PTIException">Raised if config file cannot be parsed</exception>
        public ReturnCodes SendConfiguration(BinaryFile config)
        {
            if (!_port.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }
            var configWriter = new RElConfigUpdater(this);
            return configWriter.WriteConfiguration(config);
        }

        /// <inheritdoc />
        public BinaryFile ReadConfiguration()
        {
            if (!_port.IsOpen)
            {               
                return BinaryFile.From(new byte[0]);
            }

            var configReader = new RElConfigUpdater(this);
            return configReader.ReadConfiguration();
        }

        /// <inheritdoc />
        public ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter = null)
        {
            if (!_port.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            reporter = reporter ?? new DevNullMonitor();
            var updater = new RELFwUpdater(_port, firmware)
            {
                Reporter = reporter,
                FileType = FileTypes.Base,
                RunBefore = new List<Func<ReturnCodes>> { EnterBootloader },
                RunAfter = new List<Func<ReturnCodes>> {  Reboot }
            };

            try
            {
                return updater.ExecuteUpdate();
            }
            catch (PTIException e)
            {
                reporter.ReportMessage("Flash update failed: {0}", e.Message);
                return ReturnCodes.ExecutionFailure;
            }
        }

        /// <inheritdoc />
        public Revlev GetFirmwareRevision()
        {
            if (!_port.IsOpen)
            {
                return new Revlev();
            }

            var cmd = new ReliancePacket(RelianceCommands.GetRevlev);
            // "Self" param specifies we want revlev for running application
            cmd.Add(0x10);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<Revlev>().Parse(resp);
        }

        /// <inheritdoc />
        public string GetSerialNumber()
        {
            if (!_port.IsOpen)
            {
                return string.Empty;
            }

            var cmd = new ReliancePacket(RelianceCommands.GetSerialNumber);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<PacketedString>().Parse(resp).Value;
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
            if (!_port.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            var cmd = new ReliancePacket(RelianceCommands.Ping);
            var resp = Write(cmd);
            return resp.GetPacketType() == PacketTypes.PositiveAck ? ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
        }

        /// <inheritdoc />
        /// <summary>
        /// For printers, a reboot (and any power up event) will generate a start up ticket that
        /// calibrates the paper path. Rebooting the printer remotely will cause a paper feed
        /// and leave this ticket at the front bezel. If you have auto-retract enabled, the ticket
        /// will be get pulled back into the kiosk for disposal after a period of time.
        /// </summary>
        /// <returns>Okay if printer reboots and comes online, else ExecutionFailure</returns>
        /// <exception cref="PTIException">Raised if there is an unrecoverable issue during the reboot operation. This usually
        /// means that the USB port entered an unexpected state. If this happens, dispose of this ReliancePrinter
        /// (e.g. dispose) and then try again.</exception>
        public ReturnCodes Reboot()
        {
            if (!_port.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            try
            {
                var cmd = new ReliancePacket(RelianceCommands.Reboot);
                Write(cmd);

                // Close immediately
                _port.Close();

                // Try for 7 seconds to reconnect
                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < 7000)
                {
                    Thread.Sleep(250);
                    if (_port.Open())
                    {
                        break;
                    }
                }

                return _port.IsOpen ? ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
            }
            catch (Exception e)
            {
                throw new PTIException("Error occurred during reboot: {0}", e.Message);
            }
        }

        public void Dispose()
        {
            _port?.Dispose();
        }

        /// <summary>
        /// Returns a list of installed codepage
        /// </summary>
        /// <returns>List of codepage ids or empty if none found</returns>
        internal IEnumerable<ushort> GetInstalledCodepages()
        {
            if (!_port.IsOpen)
            {
                return Enumerable.Empty<ushort>();
            }

            // 2 : get codepage list
            var raw = Write(RelianceCommands.FontSub, 2);
            var result = new List<ushort>();

            if (raw.GetPacketType() != PacketTypes.PositiveAck)
            {
                return result;
            }
            if (raw.Count < 3)
            {
                return result;
            }

            var count = raw[0];
            if (raw.Count < (count + 1))
            {
                return result;
            }
            for (var i = 1; i < (count * 2); i += 2)
            {
                result.Add((ushort)(raw[i + 1] << 8 | raw[i]));
            }

            return result;
        }

        /// <summary>
        /// Enter bootloader mode and handle the reconnection dance
        /// </summary>
        /// <returns></returns>
        internal ReturnCodes EnterBootloader()
        {
            if (!_port.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            var resp = Write(RelianceCommands.SetBootMode, 0x21);
            return resp.GetPacketType() != PacketTypes.PositiveAck ? ReturnCodes.ExecutionFailure : Reboot();
        }

        /// <summary>
        /// Write wrapper handle the write-wait-read process. The data returned
        /// from this method will be unpackaged for your.
        /// </summary>
        ///<param name="cmd">Command to send</param>
        /// <param name="data">Data to send</param>
        /// <returns>Response or empty if no response</returns>
        internal IPacket Write(RelianceCommands cmd, params byte[] data)
        {
            var packet = new ReliancePacket(cmd);           
            packet.Add(data);
            return Write(packet);
        }

        /// <summary>
        /// Write wrapper handle the write-wait-read process. The data returned
        /// from this method will be unpackaged for your.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <returns>Response or empty if no response</returns>
        internal virtual IPacket Write(IPacket data)
        {
            if (!_port.Write(data))
            {
                return _port.PacketLanguage;
            }

            var resp = _port.Read(100);
            return resp.ExtractPayload();
        }
    }
}
