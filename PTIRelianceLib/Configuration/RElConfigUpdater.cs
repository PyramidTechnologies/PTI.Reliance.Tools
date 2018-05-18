#region Header
// RElConfigUpdater.cs
// PTIRelianceLib
// Cory Todd
// 18-05-2018
// 7:41 AM
#endregion

namespace PTIRelianceLib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Protocol;
    using Transport;

    /// <summary>
    /// Handles readong from and writing to Reliance configuration space
    /// </summary>
    internal class RElConfigUpdater
    {
        private readonly ReliancePrinter _mPrinter;

        /// <summary>
        /// Create a new updater
        /// </summary>
        /// <param name="printer">Target printer</param>
        /// <exception cref="ArgumentException">Raised if printer is null</exception>
        public RElConfigUpdater(ReliancePrinter printer)
        {
            _mPrinter = printer ?? throw new ArgumentException("{0} cannot be null", nameof(printer));
        }

        public ReturnCodes WriteConfiguration(BinaryFile file)
        {
            var config = RELConfig.Load(file);
            if (config == null)
            {
                return ReturnCodes.ConfigFileInvalid;
            }

            var results = new List<ReturnCodes>();

            // Tediously populate every field
            var serialCfg = GetConfig<RELSerialConfig>(_mPrinter, RelianceCommands.GetSerialConfig);
            if (serialCfg == null)
            {
                return ReturnCodes.TargetStoppedResponding;
            }

            serialCfg.BaudRate = config.BaudRate;
            serialCfg.Databits = (byte) config.Databits;
            serialCfg.Parity = config.Parity;
            serialCfg.Stopbits = config.Stopbits;
            serialCfg.Handshake = config.Handshake;
            results.Add(SetConfig(RelianceCommands.SetSerialConfig, serialCfg));

            results.Add(SetConfig(RelianceCommands.SetPrintQuality, (byte) config.Quality));
            results.Add(SetConfig(RelianceCommands.SetRetractEnabled, (byte) (config.RetractEnabled ? 1 : 0)));
            results.Add(SetConfig(RelianceCommands.SetEjectorMode, (byte) config.Ejector));
            results.Add(SetConfig(RelianceCommands.SetTicketTimeoutPeriod, (byte) config.TicketTimeout));
            results.Add(SetConfig(RelianceCommands.SetTimeoutAction, (byte) config.TicketTimeoutAction));
            results.Add(SetConfig(RelianceCommands.SetNewTicketAction, (byte) config.NewTicketAction));
            results.Add(SetConfig(RelianceCommands.SetPresentLen, (byte) config.PresentLength));
            results.Add(SetConfig(RelianceCommands.SetCRLFConf, new PacketedBool(config.CRLFEnabled)));
            results.Add(SetConfig(RelianceCommands.SetPrintDensity, (byte) config.PrintDensity));
            results.Add(SetConfig(RelianceCommands.AutocutSub, new PacketedBool(config.AutocutEnabled), 0));
            results.Add(SetConfig(RelianceCommands.AutocutSub, 2, (byte) config.AutocutTimeout));
            results.Add(SetConfig(RelianceCommands.PaperSizeSub, 1, (byte) config.PaperWidth));

            // TODO what about maps?
            //            var installedCodepage =
            //                new List<int>() {config.Codepage1, config.Codepage2, config.Codepage3, config.Codepage4};
            //            var mapsToInstall = RelianceFontHelper.MakeFileList(installedCodepage);
            //            results.Add(_mPrinter.WriteFontMaps(mapsToInstall, new DevNullMonitor()));

            // Font configuration
            var fontInfo = new RELFont
            {
                CodePage = (ushort) config.DefaultCodepage,
                FontSize = config.FontSize,
                FontWhich = config.FontWhich,
            };
            var cps = _mPrinter.GetInstalledCodepages().ToArray();
            if (!cps.Contains(fontInfo.CodePage))
            {
                fontInfo.CodePage = cps.FirstOrDefault();
            }
            results.Add(SetConfig(RelianceCommands.FontSub, fontInfo, 1));
            results.Add(SetConfig(RelianceCommands.FontSub, 6, (byte) config.FontScalingMode));
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, new PacketedBool(config.IsCDCEnabled), 4));
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, new PacketedBool(config.IsStartupTicketEnabled), 0x0C));
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, new PacketedBool(config.IsPaperSlackEnabeld), 2));
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, new PacketedBool(config.IsUniqueUSBSNEnabled), 6));

            var xonxoff = new XonXoffConfig()
            {
                Xon = config.XonCode,
                Xoff = config.XoffCode
            };
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, xonxoff, 0x0A));

            var confrev = new ConfigRev()
            {
                Version = config.Version,
                Revision = config.Revision,
            };
            results.Add(SetConfig(RelianceCommands.GeneralConfigSub, confrev, 0x0E));


            // Bezel configuration
            var bezelconfig = RELBezel.GetDefaults();
            bezelconfig[(int) RELBezelModes.PrinterIdle].DutyCycle = config.BezelIdleDutyCycle;
            bezelconfig[(int) RELBezelModes.PrinterIdle].FlashInterval = config.BezelIdleInterval;
            bezelconfig[(int) RELBezelModes.TicketPrinting].DutyCycle = config.BezelPrintingDutyCycle;
            bezelconfig[(int) RELBezelModes.TicketPrinting].FlashInterval = config.BezelPrintingInterval;
            bezelconfig[(int) RELBezelModes.TicketPresented].DutyCycle = config.BezelPresentedDutyCycle;
            bezelconfig[(int) RELBezelModes.TicketPresented].FlashInterval = config.BezelPresentedInterval;
            bezelconfig[(int) RELBezelModes.TicketEjecting].DutyCycle = config.BezelEjectingDutyCycle;
            bezelconfig[(int) RELBezelModes.TicketEjecting].FlashInterval = config.BezelEjectingInterval;

            results.AddRange(bezelconfig.Select(relBezel => SetConfig(RelianceCommands.BezelSub, relBezel, 2)));

            // Don't forget to save these settings!
            results.Add(SetConfig(RelianceCommands.SaveConfig));

            return results.All(x => x == ReturnCodes.Okay) ? ReturnCodes.Okay : ReturnCodes.ExecutionFailure;
        }

        /// <summary>
        /// Reads configuration of printer and returns result
        /// </summary>
        /// <returns>JSON configuration</returns>
        public BinaryFile ReadConfiguration()
        {
            var config = new RELConfig();

            // TODO actually read stuff

            using (var stream = new MemoryStream())
            {
                config.Save(stream);
                return BinaryFile.From(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Writes the specified command and payload, returns a pass/fail return code
        /// </summary>
        /// <param name="cmd">Commanfd</param>
        /// <param name="parseable">Object payload</param>
        /// <param name="args">0 or more args that follow cmd before parseable payload</param>
        /// <returns>Okay or ExecutionError</returns>
        private ReturnCodes SetConfig(RelianceCommands cmd, IParseable parseable, params byte[] args)
        {
            var payload = Extensions.Concat(args, parseable.Serialize());
            return SetConfig(cmd, payload);
        }

        /// <summary>
        /// Writes the specified command and payload, returns a pass/fail return code
        /// </summary>
        /// <param name="cmd">Commanfd</param>
        /// <param name="args">0 or more bytes payload</param>
        /// <returns>Okay or ExecutionError</returns>
        private ReturnCodes SetConfig(RelianceCommands cmd, params byte[] args)
        {
            return _mPrinter.Write(cmd, args).GetPacketType() == PacketTypes.PositiveAck
                ? ReturnCodes.Okay
                : ReturnCodes.ExecutionFailure;
        }

        /// <summary>
        /// Reads the specified configuration type from printer
        /// </summary>
        /// <typeparam name="T">Type of data packet should be parsed as</typeparam>
        /// <param name="printer">Printer to query</param>
        /// <param name="cmd">Command</param>
        /// <param name="args">0 or more command arguments</param>
        /// <returns></returns>
        private static T GetConfig<T>(ReliancePrinter printer, RelianceCommands cmd, params byte[] args)
            where T : IParseable
        {
            var resp = printer.Write(cmd, args);
            return PacketParserFactory.Instance.Create<T>().Parse(resp);
        }
    }
}