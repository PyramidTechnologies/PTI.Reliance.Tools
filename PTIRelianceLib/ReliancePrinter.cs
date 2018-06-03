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
    /// Reliance Thermal Printer class provides access access to flash updating, configuration,
    /// status reporting, and other commands. For best results, we recommend testing against the
    /// latest available firmware for Reliance.
    ///
    /// Use Reliance Tools for PC to update your printer to the latest firmware through the auto
    /// update feature. You may also contact our support team to get the latest firmware file
    /// at <see href="mailto:support@pyramidacceptors.com"/>
    ///
    /// Reliance Tools for PC <see href="https://pyramidacceptors.com/app/reliance-tools/"/>
    /// </summary>
    [DebuggerDisplay("IsOpen = {_mPort.IsOpen}")]
    public class ReliancePrinter : IPyramidDevice
    {
        /// <summary>
        /// USB vendor id for all Reliance USB interfaces
        /// </summary>
        /// <value>USB VID</value>
        public const int VendorId = 0x0425;
        /// <summary>
        /// USB product id for all Reliance USB interfaces
        /// </summary>
        /// <value>USB PID</value>
        public const int ProductId = 0x8147;

        /// <summary>
        /// Underlying communication handle
        /// </summary>
        private IPort<IPacket> _mPort;

        /// <summary>
        /// HID configuration parameters for Reliance
        /// </summary>
        private readonly HidDeviceConfig _mPortConfig;

        /// <summary>
        /// Create a new Reliance Printer. The printer will be discovered automatically. If HIDapi
        /// or one of its depencies cannot be found or loaded, <exception cref="PTIException"></exception>
        /// will be thrown.
        /// </summary>
        /// <exception cref="PTIException">Thrown if native HID library cannot be loaded</exception> 
        public ReliancePrinter()
        {
            // Reliance will "always" use report lengths of 34 bytes
            _mPortConfig = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 34,
                OutReportLength = 34,
                InReportId = 2,
                OutReportId = 1,
                NativeHid = new NativeMethods()
            };

            MakeNewPort();
        }

        private IPort<IPacket> MakeNewPort()
        {
            try
            {
                _mPort?.Dispose();         
                _mPort = new HidPort<ReliancePacket>(_mPortConfig);
            }
            catch (DllNotFoundException ex)
            {
                // Re throw as our own exception
                throw new PTIException("Failed to load HID library: {0}", ex.Message);
            }

            return _mPort;
        }

        /// <summary>
        /// Internal constructor using specified HID configurations from
        /// <see cref="HidDeviceConfig"/>.
        /// </summary>
        /// <param name="config">HID library options</param>
        internal ReliancePrinter(HidDeviceConfig config)
        {
            _mPortConfig = config;
            MakeNewPort();
        }

        /// <inheritdoc />
        /// <summary>
        /// Parses <paramref name="config"/> from <see cref="BinaryFile"/> and
        /// sends to target printer. <exception cref="PTIException"/> is thrown if config file cannot be parsed
        /// </summary>
        /// <param name="config">Configuration to send</param>
        /// <exception cref="PTIException">Thrown if config file cannot be parsed</exception> 
        public ReturnCodes SendConfiguration(BinaryFile config)
        {
            if (!_mPort.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }
            var configWriter = new RElConfigUpdater(this);
            return configWriter.WriteConfiguration(config);
        }

        /// <inheritdoc />
        public BinaryFile ReadConfiguration()
        {
            if (!_mPort.IsOpen)
            {               
                return BinaryFile.From(new byte[0]);
            }

            var configReader = new RElConfigUpdater(this);
            return configReader.ReadConfiguration();
        }

        /// <inheritdoc />
        ///  <remarks>
        ///  Updates target Reliance Thermal printer with this firmware.
        ///  This API supports printers with firmware <c>1.22</c> or newer. If you
        ///  have a printer firmware older than <c>1.22</c>, you must use the PC
        ///  version of Reliance Tools to upgrade your firmware.
        ///  </remarks>       
        /// <returns>
        ///<list type="bullet">
        ///<item>
        /// <description><see cref="ReturnCodes.Okay"/> when update is successful</description>
        /// </item>
        ///<item>
        /// <description><see cref="ReturnCodes.FlashInstalledFwNotSupported"/> if your
        /// printer has too old of firmware.</description>
        /// </item>
        ///<item>         
        /// <description>Another <see cref="ReturnCodes"/> if update fails</description>
        /// </item>
        /// </list>
        /// </returns>
        public ReturnCodes FlashUpdateTarget(BinaryFile firmware, ProgressMonitor reporter = null)
        {
            if (!_mPort.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            var revlev = GetFirmwareRevision();
            if (revlev < new Revlev(1, 22, 0))
            {
                // We support all bootloader versions
                var appid = GetAppId();
                if (!appid.ToLower().Equals("bootloader"))
                {
                    return ReturnCodes.FlashInstalledFwNotSupported;
                }
            }

            reporter = reporter ?? new DevNullMonitor();
            var updater = new RELFwUpdater(_mPort, firmware)
            {
                Reporter = reporter,
                FileType = FileTypes.Base,
                RecoverConnection = MakeNewPort,
                RunBefore = new List<Func<ReturnCodes>> { EnterBootloader },
                RunAfter = new List<Func<ReturnCodes>> {  Reboot }
            };

            try
            {
                var resp = updater.ExecuteUpdate();

                // Make sure port is in a usuable state
                MakeNewPort();

                return resp;
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
            if (!_mPort.IsOpen)
            {
                return new Revlev();
            }

            var cmd = new ReliancePacket(RelianceCommands.GetRevlev);
            // "Self" param specifies we want revlev for running application
            cmd.Add(0x10);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<Revlev>().Parse(resp);
        }

        /// <summary>
        /// Returns the <see cref="Status"/> for the attached printer. If there
        /// is no device connected, <c>null</c> will be returned.
        /// </summary>
        /// <returns>Status object or <c>null</c> if no connection or error</returns>
        public Status GetStatus()
        {
            if (!_mPort.IsOpen)
            {
                return null;
            }

            var cmd = new ReliancePacket(RelianceCommands.GetPrinterStatus);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<Status>().Parse(resp);
        }

        /// <inheritdoc />
        public string GetSerialNumber()
        {
            if (!_mPort.IsOpen)
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
        /// If <see cref="ReliancePrinter.Ping()"/> is called during a print, this will block for a small period
        /// of time and then return unsuccessfully. The resulting ReturnCode will be
        /// <see cref="ReturnCodes.ExecutionFailure"/>. It is recommended to try this method multiple times before
        /// assuming the printer is offline.
        /// </summary>
        public ReturnCodes Ping()
        {
            if (!_mPort.IsOpen)
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
        /// will be get pulled back into the kiosk for disposal after a period of time. An exception
        /// of type <exception cref="PTIException"/> may be raised if there is an error during reboot.
        /// </summary>
        /// <exception cref="PTIException">Raised if there is an unrecoverable issue during the reboot operation. This usually
        /// means that the USB port entered an unexpected state. If this happens, dispose of this ReliancePrinter
        /// (e.g. dispose) and then try again.</exception>
        public ReturnCodes Reboot()
        {
            if (!_mPort.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            try
            {
                var cmd = new ReliancePacket(RelianceCommands.Reboot);
                Write(cmd);

                // Close immediately
                _mPort = null;

                // Try for XX seconds to reconnect
                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < 10000)
                {
                    Thread.Sleep(250);

                    MakeNewPort();

                    if (_mPort?.Open() == true)
                    {
                        break;
                    }
                }

                return _mPort?.IsOpen == true ? ReturnCodes.Okay : ReturnCodes.RebootFailure;
            }
            catch (Exception e)
            {
                throw new PTIException("Error occurred during reboot: {0}", e.Message);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _mPort?.Dispose();
        }

        /// <summary>
        /// Returns a list of installed codepages. This is a list of
        /// codepage ids that are installed on the target printer. Only
        /// codepages that are installed may be selected for usage.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item><description>IEnumerable of <c>ushort</c> codepage IDs</description></item>
        /// <item><description>Empty IEnumerable of <c>ushort</c> on error</description></item>
        /// </list>
        /// </returns>
        public IEnumerable<ushort> GetInstalledCodepages()
        {
            if (!_mPort.IsOpen)
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
        /// Returns firmware application id string or empty on error
        /// </summary>
        /// <returns>Application id string</returns>
        internal string GetAppId()
        {
            if (!_mPort.IsOpen)
            {
                return string.Empty;
            }

            var cmd = new ReliancePacket((byte)RelianceCommands.GetBootId, 0x10);
            var resp = Write(cmd);
            return PacketParserFactory.Instance.Create<PacketedString>().Parse(resp).Value;
        }

        /// <summary>
        /// Enter bootloader mode and handle the reconnection dance
        /// </summary>
        /// <returns></returns>
        internal ReturnCodes EnterBootloader()
        {
            if (!_mPort.IsOpen)
            {
                return ReturnCodes.DeviceNotConnected;
            }

            var resp = Write(RelianceCommands.SetBootMode, 0x21);
            return resp.GetPacketType() != PacketTypes.PositiveAck ? ReturnCodes.FailedBootloaderEntry : Reboot();
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
            if (!_mPort.Write(data))
            {
                return _mPort.PacketLanguage;
            }

            var resp = _mPort.Read(100);
            return resp.ExtractPayload();
        }
    }
}
