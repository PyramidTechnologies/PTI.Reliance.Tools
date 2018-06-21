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
        /// <exception cref="ArgumentException">Raised if printer is <c>null</c></exception>
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
           
            // Position in array needs to be used in the payload
            for (byte i = 0; i < bezelconfig.Count; ++i)
            {
                // [bezel sub] [2] [printer state (i)] [ ... payload ... ]
                results.Add(SetConfig(RelianceCommands.BezelSub, bezelconfig[i], 2, i));
            }

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
            if (!FillSerialConfig(ref config))
            {
                return BinaryFile.From(new byte[0]);
            }

            FillGeneralConfig(ref config);
            FillFontConfig(ref config);
            FillBezelConfig(ref config);

            using (var stream = new MemoryStream())
            {
                config.Save(stream);
                return BinaryFile.From(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Reads in and fill config with all general parameters
        /// </summary>
        /// <param name="config">Configuration to fill</param>
        private void FillGeneralConfig(ref RELConfig config)
        {
            config.Quality = (ReliancePrintQuality)(GetConfig(_mPrinter, RelianceCommands.GetPrintQuality) ?? 0);
            config.RetractEnabled = GetConfig(_mPrinter, RelianceCommands.GetRetractEnabled) == 1;
            config.Ejector = (RelianceEjectorMode)(GetConfig(_mPrinter, RelianceCommands.GetEjectorMode) ?? 0);

            config.TicketTimeout = GetConfig(_mPrinter, RelianceCommands.GetTicketTimeoutPeriod) ?? 5;
            config.TicketTimeoutAction = (TicketTimeoutAction)(GetConfig(_mPrinter, RelianceCommands.GetTimeoutAction) ?? 0);
            config.NewTicketAction = (NewTicketAction)(GetConfig(_mPrinter, RelianceCommands.GetNewTicketAction) ?? 1);
            config.PresentLength = GetConfig(_mPrinter, RelianceCommands.GetPresentLen) ?? 48;
            config.CRLFEnabled = GetConfig(_mPrinter, RelianceCommands.GetCRLFConf) == 1;
            config.PrintDensity = GetConfig(_mPrinter, RelianceCommands.GetPrintDensity) ?? 100;
            config.AutocutEnabled = GetConfig(_mPrinter, RelianceCommands.AutocutSub, 1) == 1;
            config.AutocutTimeout = GetConfig(_mPrinter, RelianceCommands.AutocutSub, 3) ?? 1;
            config.PaperWidth = PaperSizeUtils.FromByte((byte)(GetConfig(_mPrinter, RelianceCommands.PaperSizeSub, 0) ?? 80));

            var configrev = GetConfig<ConfigRev>(_mPrinter, RelianceCommands.GeneralConfigSub, 0x0F);
            config.Version = configrev.Version;
            config.Revision = configrev.Revision;
        }

        /// <summary>
        /// Reads in and fill config with all bezel parameters
        /// </summary>
        /// <param name="config">Configuration to fill</param>
        private void FillBezelConfig(ref RELConfig config)
        {
            var bezels = new List<RELBezel>();
            for (byte i = 0; i < 4; ++i)
            {
                // [bezel sub] [3] [printer state (i)]
                bezels.Add(GetConfig<RELBezel>(_mPrinter, RelianceCommands.BezelSub, 3, i));
            }

            config.BezelIdleDutyCycle = bezels[0].DutyCycle;
            config.BezelIdleInterval = bezels[0].FlashInterval;
            config.BezelPrintingDutyCycle = bezels[1].DutyCycle;
            config.BezelPrintingInterval = bezels[1].FlashInterval;
            config.BezelPresentedDutyCycle = bezels[2].DutyCycle;
            config.BezelPresentedInterval = bezels[2].FlashInterval;
            config.BezelEjectingDutyCycle = bezels[3].DutyCycle;
            config.BezelEjectingInterval = bezels[3].FlashInterval;
        }

        /// <summary>
        /// Reads in and fills config with all font parameters
        /// </summary>
        /// <param name="config">Configuration to fill</param>
        private void FillFontConfig(ref RELConfig config)
        {
            var font = GetConfig<RELFont>(_mPrinter, RelianceCommands.FontSub, 0);
            config.FontWhich = font.FontWhich;
            config.FontSize = font.FontSize;
            config.DefaultCodepage = font.CodePage;
            config.FontScalingMode = (RelianceScalarMode)(GetConfig(_mPrinter, RelianceCommands.FontSub, 7) ?? 1);

            var codepages = _mPrinter.GetInstalledCodepages().ToList();
            config.Codepage1 = codepages.Count > 0 ? codepages[0] : font.CodePage;
            config.Codepage2 = codepages.Count > 1 ? codepages[1] : 0;
            config.Codepage3 = codepages.Count > 2 ? codepages[2] : 0;
            config.Codepage4 = codepages.Count > 3 ? codepages[3] : 0;

            config.IsCDCEnabled = GetConfig(_mPrinter, RelianceCommands.GeneralConfigSub, 5) == 1;
            config.IsStartupTicketEnabled = GetConfig(_mPrinter, RelianceCommands.GeneralConfigSub, 0x0D) == 1;
            config.IsPaperSlackEnabeld = GetConfig(_mPrinter, RelianceCommands.GeneralConfigSub, 3) == 1;
            config.IsUniqueUSBSNEnabled = GetConfig(_mPrinter, RelianceCommands.GeneralConfigSub, 7) == 1;

        }

        /// <summary>
        /// Reads Serial configuration into config reference
        /// </summary>
        /// <param name="config">Configuration to fill</param>
        /// <returns>True on succes, else false</returns>
        private bool FillSerialConfig(ref RELConfig config)
        {
            var serial = GetConfig<RELSerialConfig>(_mPrinter, RelianceCommands.GetSerialConfig);
            if (serial == null)
            {
                // Read failure
                return false;
            }

            config.BaudRate = serial.BaudRate;
            config.Handshake = serial.Handshake;
            config.Databits = serial.Databits;
            config.Stopbits = serial.Stopbits;
            config.Parity = serial.Parity;


            var xonxoff = GetConfig<XonXoffConfig>(_mPrinter, RelianceCommands.GeneralConfigSub, 0x0B);
            config.XonCode = xonxoff.Xon;
            config.XoffCode = xonxoff.Xoff;

            return true;
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

        /// <summary>
        /// Reads generic configuration code as an integer.
        /// </summary>
        /// <param name="printer">Target device</param>
        /// <param name="cmd">Control code</param>
        /// <param name="args">0 or more args</param>
        /// <returns>Integer code or <c>null</c> on error</returns>
        private static int? GetConfig(ReliancePrinter printer, RelianceCommands cmd, params byte[] args)
        {
            var resp = printer.Write(cmd, args);
            if (resp.GetPacketType() != PacketTypes.PositiveAck)
            {
                return null;
            }

            var payload = resp.GetBytes();
            switch (payload.Length)
            {
                case 1:
                    return payload[0];
                case 2:
                    return payload.ToUshortBE();
                case 4:
                    return payload.ToUshortBE();
                default:
                    return null;
            }

        }
    }
}