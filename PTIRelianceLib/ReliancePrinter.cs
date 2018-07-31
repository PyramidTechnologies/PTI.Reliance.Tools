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
    using Firmware;
    using Imaging;
    using IO;
    using IO.Internal;
    using Logging;
    using Logo;
    using Protocol;
    using Telemetry;
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
        private static readonly ILog Log = LogProvider.For<ReliancePrinter>();

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

        /// <inheritdoc />
        /// <summary>
        /// Create a new Reliance Printer. The printer will be discovered automatically. If HIDapi
        /// or one of its depencies cannot be found or loaded, <exception cref="T:PTIRelianceLib.PTIException"></exception>
        /// will be thrown. This constructor returns the first available Reliance printer without
        /// filtering for device path.
        /// </summary>
        /// <exception cref="T:PTIRelianceLib.PTIException">Thrown if native HID library cannot be loaded</exception> 
        public ReliancePrinter() : this(string.Empty)
        {
        }


        /// <summary>
        /// Create a new Reliance Printer. The printer will be discovered automatically. If HIDapi
        /// or one of its dependencies cannot be found or loaded, <exception cref="PTIException"></exception>
        /// will be thrown. This constructor supports filtering for devices by hardware path. The device path
        /// can be obtained from your operating system or by checking <see cref="DevicePath"/>
        /// after a successful connection.
        /// </summary>
        /// <param name="devicePath">OS-dependencet device path to attach to. This is optional
        /// and may be left null or empty to accept any Reliance connection, see
        /// <see cref="DevicePath"/></param>
        /// <exception cref="PTIException">Thrown if native HID library cannot be loaded</exception> 
        public ReliancePrinter(string devicePath)
        {
            // Reliance will "always" use report lengths of 34 bytes
            _mPortConfig = new HidDeviceConfig
            {
                VendorId = VendorId,
                ProductId = ProductId,
                InReportLength = 35,
                OutReportLength = 35,
                InReportId = 2,
                OutReportId = 1,
                NativeHid = new NativeMethods(),
                DevicePath = devicePath
            };

            AcquireHidPort();
        }
        
        /// <inheritdoc />
        public bool IsDeviceReady => _mPort?.IsOpen ?? false;
        
        /// <summary>
        public string DevicePath => _mPort.IsOpen ? _mPort.PortPath : string.Empty;

        /// <summary>
        /// Internal constructor using specified HID configurations from
        /// <see cref="HidDeviceConfig"/>.
        /// </summary>
        /// <param name="config">HID library options</param>
        internal ReliancePrinter(HidDeviceConfig config)
        {
            _mPortConfig = config;
            AcquireHidPort();
        }

        /// <summary>
        /// Closes current port if possible. A new port is created and an attempt to connect is made.
        /// The _mPort field will non-null as long as the native HID library is loaded. The _mPort
        /// value is returned as a convenience.
        /// A <see cref="PTIException"/> is thrown if the native shared HID library cannot be loaded
        /// </summary>
        /// <exception cref="PTIException">Thrown if native HID library cannot be loaded</exception>
        /// <returns>IPort or null on error</returns>
        /// <value>IPort instance or null</value>
        private IPort<IPacket> AcquireHidPort()
        {
            try
            {
                _mPort?.Close();
                _mPort = new HidPort<ReliancePacket>(_mPortConfig);
            }
            catch (DllNotFoundException ex)
            {
                // Re throw as our own exception
                throw new PTIException("Failed to load HID library: {0}", ex.Message);
            }

            return _mPort;
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

            var firmwareRevision = GetFirmwareRevision();
            if (firmwareRevision < new Revlev(1, 22, 0))
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
                RecoverConnection = AcquireHidPort,
                RunBefore = new List<Func<ReturnCodes>>(),
                RunAfter = new List<Func<ReturnCodes>> {Reboot, CheckChecksum}
            };

            // If not already in bootloader mode, do so before flash update
            var appID = GetAppId();
            if (!appID.ToLower().Equals("bootloader"))
            {
                updater.RunBefore.Add(EnterBootloader);
            }

            try
            {
                var resp = updater.ExecuteUpdate();

                // Make sure port is in a usable state
                AcquireHidPort();

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

            Log.Debug("Requesting revision level");

            // 0x10: specifies we want firmware revision for running application
            var cmd = _mPort.Package((byte) RelianceCommands.GetRevlev, 0x10);

            var resp = Write(cmd);
            var rev = PacketParserFactory.Instance.Create<Revlev>().Parse(resp);

            Log.Debug("Found firmware revision {0}", rev);
            return rev;
        }

        /// <summary>
        /// Returns the <see cref="Status"/> for the attached printer. If there
        /// is no device connected, <c>null</c> will be returned.
        /// You can use this method to read the ticket status. See <seealso cref="TicketStates"/>,
        /// which are a member of <seealso cref="Status"/>.
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

            Log.Debug("Sending Ping");

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
                Log.Debug("Rebooting printer");
                var cmd = new ReliancePacket(RelianceCommands.Reboot);
                Write(cmd);

                var retry = 0;
                while (++retry < 10)
                {
                    // Calls port close which has delay baked in
                    AcquireHidPort();

                    if (Library.Options.HidReconnectDelayMs > 0)
                    {
                        Thread.Sleep(Library.Options.HidReconnectDelayMs);
                    }

                    if (_mPort?.IsOpen == true)
                    {
                        break;
                    }

                    Log.Trace("Reboot reconnect attempt: {0}", retry);
                }

                if (!_mPort?.IsOpen != true)
                {
                    return ReturnCodes.Okay;
                }

                Log.Warn("Failing to reboot indicates that Reliance is having trouble reconnecting to your hardware." +
                         "It is recommended to check PTIRelianceLib.Library.Options for configuration options.");
                return ReturnCodes.RebootFailure;
            }
            catch (Exception e)
            {
                throw new PTIException("Error occurred during reboot: {0}", e.Message);
            }
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
                result.Add((ushort) (raw[i + 1] << 8 | raw[i]));
            }

            return result;
        }

        /// <summary>
        /// Writes the provided logos to internal logo bank. These logos can be accessed
        /// by their index in the list using <see cref="PrintLogo"/>. Logos are stored in
        /// non-volatile memory so these only need to be written once. It is not recommended
        /// to call this method excessively (e.g. on every print job) as this is writing to
        /// flash and you will degrade the flash chip.
        /// <example>        
        /// myLogos = { logoA, logoB, logoC };
        /// StoreLogos(myLogos, monitor, storageConfig);
        /// printer.PrintLogo(0); // Print logo A
        /// printer.PrintLogo(2); // Print logo C
        /// </example>
        /// LogoStorageConfig can be used to alter the dithering algorithm and logo scaling behavior.
        /// </summary>
        /// <param name="logoData">List of raw logo image data</param>
        /// <param name="monitor">Progress reporter</param>        
        /// <param name="storageConfig">Logo alteration and storage options.
        /// Uses <seealso cref="LogoStorageConfig.Default"/> if storageConfig is set to null.
        /// You probably want to configure this yourself.</param>
        /// <returns>Return Code</returns>
        /// <exception cref="ArgumentNullException">Thrown if logo data is null</exception>
        public ReturnCodes StoreLogos(IList<BinaryFile> logoData, IProgressMonitor monitor,
            LogoStorageConfig storageConfig)
        {
            if (logoData == null)
            {
                throw new ArgumentNullException(nameof(logoData));
            }

            if (storageConfig == null)
            {
                storageConfig = LogoStorageConfig.Default;
            }

            var result = ReturnCodes.InvalidRequest;

            if (logoData.Count == 0)
            {
                return result;
            }

            var logoBank = new RELLogoBank();
            var ditheredLogos = logoData.Select(logo =>
                new BasePrintLogo(logo, maxWidth: storageConfig.MaxPixelWidth)).Cast<IPrintLogo>().ToList();
            ditheredLogos.ForEach(x => x.ApplyDithering(storageConfig.Algorithm, storageConfig.Threshold));

            // MakeHeaders locks in our scaling and dithering options
            foreach (var header in logoBank.MakeHeaders(ditheredLogos))
            {
                // sub command 2: Set Logo header
                var cmd = _mPort.Package((byte) RelianceCommands.LogoSub, 0x02);
                cmd.Add(header.Serialize());

                // Send the data and parse response
                var resp = Write(cmd);
                if (resp.GetPacketType() != PacketTypes.PositiveAck)
                {
                    Log.ErrorFormat("Failed to write logo header: {0}", resp.GetPacketType());
                    return ReturnCodes.ExecutionFailure;
                }

                // Specify start address to that when the big logo buffer is built,
                // the correct address is injected in the flash permission command
                var logoFlasher = new RELLogoUpdater(_mPort, header)
                {
                    Reporter = monitor,
                };

                result = logoFlasher.ExecuteUpdate();
                if (result == ReturnCodes.Okay)
                {
                    continue;
                }

                Log.ErrorFormat("Failed to write logo: {0}", result.GetEnumName());
                return result;
            }

            if (result == ReturnCodes.Okay)
            {
                // Store logo header data
                Write(RelianceCommands.SaveConfig);
            }

            return result;
        }

        /// <summary>
        /// Prints logo from internal bank specified by index. If the specified
        /// logo index does not exist, the first logo in the bank will instead be printed.
        /// Negative indices and indices larger than 255 will generate a
        /// <see cref="ReturnCodes.InvalidRequestPayload"/> return code.
        /// </summary>
        /// <param name="index">Logo index, zero based</param>
        /// <returns>Return Code</returns>
        public ReturnCodes PrintLogo(int index)
        {
            if (index < 0 || index > byte.MaxValue)
            {
                return ReturnCodes.InvalidRequestPayload;
            }

            // 7 == print logo sub command
            Write(RelianceCommands.LogoSub, 7, (byte) index);
            return ReturnCodes.Okay;
        }

        /// <summary>
        /// Returns the telemetry data over the lifetime of this printer
        /// </summary>
        /// <remarks>Requires firmware 1.28+. Older firmware will result in null result.</remarks>
        /// <returns>LifetimeTelemetry data since printer left factory</returns>
        public LifetimeTelemetry GetLifetimeTelemetry()
        {
            if (GetFirmwareRevision() < new Revlev("1.28"))
            {
                // unsupported
                return null;
            }

            // 1: read non-volatile version, from start to end
            return (LifetimeTelemetry) ReadTelemetry(TelemetryTypes.Lifetime);
        }

        /// <summary>
        /// Returns the telemetry data of this printer since last power up        
        /// </summary>
        /// <remarks>Requires firmware 1.28+. Older firmware will result in null result.</remarks>
        /// <returns>Powerup telemetry or null if read failure or unsupported firmware</returns>       
        public PowerupTelemetry GetPowerupTelemetry()
        {
            // 2: read volatile version, from start to end
            return GetFirmwareRevision() < new Revlev("1.28") ? null : ReadTelemetry(TelemetryTypes.Powerup);
        }

        /// <summary>
        /// Reset all telemetry counters. This include the lifetime and powerup counters.
        /// This operation cannot be undone.
        /// </summary>
        /// <remarks>Data will be permanently erased</remarks>
        /// <returns>Success code</returns>
        public ReturnCodes ResetTelemetry()
        {
            var resp = Write(RelianceCommands.TelemtrySub, 0x03);
            return resp.GetPacketType() == PacketTypes.PositiveAck ? ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
        }

        /// <summary>
        /// Write raw data to device and return response. Only valid responses will be returned.
        /// If the response is malformed or otherwise not intact, the data will be discarded and 
        /// the result will be an empty array.
        ///
        /// This data will be passed to the configuration interface of the target device. It will
        /// not be passed to the print buffer in anyway. If you are looking to print data, this
        /// cannot be done using this API.
        /// </summary>
        /// <param name="cmd">Command byte and parameters to send to target</param>
        /// <param name="args">Optional arguments to command, up to 32 bytes</param>
        /// <returns>Response data or empty buffer is reponse is malformed. If more than 32 bytes are passed
        /// as an argument, the command will be aborted and an empty buffer will be returned.</returns>
        protected byte[] WriteRaw(byte cmd, params byte[] args)
        {
            var payload = _mPort.Package(cmd);
            if (args.Length >= 32)
            {
                return new byte[0];
            }

            foreach (var arg in args)
            {
                payload.Add(arg);
            }

            var resp = Write(payload);
            switch (resp.GetPacketType())
            {
                case PacketTypes.Normal:
                    return resp.GetBytes();
                case PacketTypes.PositiveAck:
                    return new byte[] {0xAA};
                case PacketTypes.NegativeAck:
                    return new byte[] {0xAC};
                default:
                    return new byte[0];
            }
        }

        /// <summary>
        /// Reads the specified telemetry data block
        /// </summary>
        /// <param name="type">Type of telemetry to request</param>
        /// <returns></returns>
        internal PowerupTelemetry ReadTelemetry(TelemetryTypes type)
        {
            Log.Debug("Requesting telemetry info");

            var readLen = ReadTelemetrySize(type);
            if (readLen <= 0)
            {
                // Bad read size
                return null;
            }

            // Build permission request. 0: request data
            var request = _mPort.Package((byte) RelianceCommands.TelemtrySub, 0, (byte) type);

            // Read entire data chunk from start of data (0->readLen)
            request.Add(((ushort) 0).ToBytesBE());
            request.Add(((ushort) readLen).ToBytesBE());

            // Request permission
            var resp = Write(request);
            if (resp.GetPacketType() != PacketTypes.PositiveAck)
            {
                // Permission denied
                return null;
            }

            // 1: repeatedly read data request
            var preamble = new byte[] {(byte) RelianceCommands.TelemtrySub, 1};

            // Execute a structured read
            var reader = new StructuredReader(_mPort);
            resp = reader.Read(readLen, preamble);
            return PacketParserFactory.Instance.Create<LifetimeTelemetry>().Parse(resp);
        }

        /// <summary>
        /// Reads the size of the specified telemetry struct from target
        /// </summary>
        /// <param name="type">Telemetry format to request sizeof</param>
        /// <returns>Integer size in bytes, -1 on error</returns>
        internal int ReadTelemetrySize(TelemetryTypes type)
        {
            // 4: read data size request
            var getSize = _mPort.Package((byte) RelianceCommands.TelemtrySub, 4, (byte) type);
            var resp = Write(getSize);
            if (resp.GetPacketType() != PacketTypes.PositiveAck)
            {
                return -1;
            }

            var readLen = PacketParserFactory.Instance.Create<PacketedShort>().Parse(resp).Value;
            if (readLen <= 0)
            {
                // Invalid data read length
                return -1;
            }

            return readLen;
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

            Log.Debug("Requesting app id");
            var cmd = new ReliancePacket((byte) RelianceCommands.GetBootId, 0x10);
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

            Log.Debug("Entering bootloader");
            var resp = Write(RelianceCommands.SetBootMode, 0x21);

            return resp.GetPacketType() != PacketTypes.PositiveAck ? ReturnCodes.FailedBootloaderEntry : Reboot();
        }


        /// <summary>
        /// Check checksum of target device. If checksum is okay,
        /// return is <see cref="ReturnCodes.Okay"/>. Otherwise
        /// <see cref="ReturnCodes.FlashChecksumMismatch"/> is returned.
        /// </summary>
        /// <returns>Return code</returns>
        internal ReturnCodes CheckChecksum()
        {
            // Get expected checksum
            var resp = Write(RelianceCommands.GetExpectedCsum, 0x11);
            var expectedChecksum = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(resp);

            // Check actual checksum
            resp = Write(RelianceCommands.GetActualCsum, 0x11);
            var actualChecksum = PacketParserFactory.Instance.Create<PacketedInteger>().Parse(resp);

            return Equals(expectedChecksum, actualChecksum) ? ReturnCodes.Okay : ReturnCodes.FlashChecksumMismatch;
        }

        /// <summary>
        /// Write wrapper handle the write-wait-read process. The data returned
        /// from this method will be un-packaged for your.
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
        /// from this method will be payload portion of the HID packet.
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

        /// <inheritdoc />
        public void Dispose()
        {
            _mPort?.Close();
        }
    }
}