﻿namespace RelianceCLI
{
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using PTIRelianceLib.Imaging;
    using PTIRelianceLib.Protocol;
    using System;
    using System.Collections.Generic;
    using PTIRelianceLib;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Github issue dotnet/corefx#23608
            new ArgumentException();

            Console.WriteLine("Connected to PTIRelianceLib Version: {0}", PTIRelianceLib.Library.Version);

            var opts = Options.Parse(args);

            if (opts.Okay)
            {
                try
                {
                    Run(opts);
                }
                catch (PTIException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine(opts.Message);
                Console.WriteLine(Options.Usage());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// Executes primary application.        
        /// </summary>
        /// <param name="opts">Program options</param>
        /// <exception cref="PTIException">Raised if native HID library cannot be found</exception>
        private static void Run(Options opts)
        {
            Console.WriteLine("Testing HID interop");

            using (var printer = new ReliancePrinter())
            {
                var ping = printer.Ping();
                Console.WriteLine("Ping: {0}", ping);

                if (opts.Reboot)
                {
                    Console.WriteLine("Reboot: {0}", printer.Reboot());
                }

                if (opts.GetRevlev)
                {
                    var revlev = printer.GetFirmwareRevision();
                    Console.WriteLine("Revision: {0}", revlev);
                }
                
                if (opts.GetType)
                {
                    var type = printer.GetPrinterType();

                    if (type is null)
                    {
                        Console.WriteLine("Unable to read printer type.");
                    }
                    else
                    {
                        Console.WriteLine($"Printer type is {type}");
                    }
                }

                if (!string.IsNullOrEmpty(opts.FirmwareFilePath))
                {
                    Console.WriteLine("Selected new firmware: {0}", opts.FirmwareFilePath);
                    var file = BinaryFile.From(opts.FirmwareFilePath);

                    if (file.Empty)
                    {
                        Console.WriteLine("Firmware file cannot be read. Does it exist?");
                    }
                    else
                    {
                        // You could also use a DevNullMonitor if you want to ignore output entirely
                        var monitor = new ProgressMonitor();
                        monitor.OnFlashMessage += (s, o) => Console.WriteLine("\n{0}", o.Message);

                        // Simple progress monitor
                        monitor.OnFlashProgressUpdated += (s, o) =>
                        {
                            Console.CursorLeft = 0;
                            Console.CursorVisible = false;
                            Console.Write("{0:0.00}%", o.Progress * 100);
                        };

                        // Do the update!
                        var result = printer.FlashUpdateTarget(file, monitor);

                        Console.WriteLine("\nFlash Update Result: {0}", result);
                    }

                }

                if (!string.IsNullOrEmpty(opts.ConfigFilePath))
                {
                    Console.WriteLine("Selected new configuration: {0}", opts.ConfigFilePath);
                    var file = BinaryFile.From(opts.ConfigFilePath);

                    if (file.Empty)
                    {
                        Console.WriteLine("Configuration file cannot be read. Does it exist?");
                    }
                    else
                    {
                        var result = printer.SendConfiguration(file);

                        Console.WriteLine("Result: {0}", result);

                        printer.Reboot();
                    }
                }

                if (!string.IsNullOrEmpty(opts.SaveConfigPath))
                {
                    var config = printer.ReadConfiguration();
                    File.WriteAllBytes(opts.SaveConfigPath, config.GetData());
                    Console.WriteLine("Read and saved printer configuration to {0}", opts.SaveConfigPath);
                }

                if (!string.IsNullOrEmpty(opts.LogoFilePath))
                {
                    var logo = BinaryFile.From(opts.LogoFilePath);
                    if (logo.Empty)
                    {
                        Console.WriteLine("Logo file cannot be read. Does it exist?");
                    }
                    else
                    {
                        var logos = new List<BinaryFile> {logo};
                        var config = LogoStorageConfig.Default;
                        config.Algorithm = DitherAlgorithms.Atkinson;

                        var result = printer.StoreLogos(logos, new ConsoleProgressBar(), config);
                        Console.WriteLine("Store Result: {0}", result);

                        printer.PrintLogo(0);
                    }
                }

                if (opts.LifetimeTelemetry)
                {
                    var resp = printer.GetLifetimeTelemetry();
                    var str = JsonConvert.SerializeObject(resp, Formatting.Indented, new StringEnumConverter());
                    Console.WriteLine("Lifetime Telementry:\n{0}", str);
                }

                if (opts.StartupTelemetry)
                {
                    var resp = printer.GetPowerupTelemetry();
                    var str = JsonConvert.SerializeObject(resp, Formatting.Indented, new StringEnumConverter());
                    Console.WriteLine("Powerup Telementry:\n{0}", str);
                }

                if (opts.GetStatus)
                {
                    var status = printer.GetStatus();
                    Console.WriteLine("Printer status:\n{0}", status);
                    Console.WriteLine("Has Paper? :{0}", status.SensorStatus.HasFlag(SensorStatuses.Paper));

                }
            }
        }

        private struct Options
        {
            public bool Okay;

            public string Message;

            public string FirmwareFilePath;

            public string ConfigFilePath;

            public string LogoFilePath;

            public bool Reboot;

            public bool GetRevlev;

            public bool GetType;

            public bool GetStatus;

            public string SaveConfigPath;

            public bool LifetimeTelemetry;

            public bool StartupTelemetry;

            public static Options Parse(IEnumerable<string> args)
            {
                var opts = new Options();

                Action<string> nextCapture = null;
                var lastSwitch = string.Empty;

                foreach (var str in args)
                {
                    if (nextCapture != null)
                    {
                        nextCapture(str);
                        nextCapture = null;
                    }
                    else
                    {
                        switch (str)
                        {
                            case "-p":
                            case "--power":
                                opts.Reboot = true;
                                break;
                            case "-f":
                            case "--firmware":
                                lastSwitch = str;
                                nextCapture = s => opts.FirmwareFilePath = s;
                                break;

                            case "-c":
                            case "--config":
                                lastSwitch = str;
                                nextCapture = s => opts.ConfigFilePath = s;
                                break;

                            case "-r":
                            case "--revision":
                                opts.GetRevlev = true;
                                break;
                            
                            case "-pt":
                            case "--type":
                                opts.GetRevlev = true;
                                break;

                            case "-l":
                            case "--logo":
                                lastSwitch = str;
                                nextCapture = s => opts.LogoFilePath = s;
                                break;

                            case "-s":
                            case "--status":
                                opts.GetStatus = true;
                                break;

                            case "-g":
                            case "--get-config":
                                lastSwitch = str;
                                nextCapture = s => opts.SaveConfigPath = s;
                                break;

                            case "-t":
                            case "--startup-telem":
                                opts.StartupTelemetry = true;
                                break;
                            case "-T":
                            case "--lifetime-telem":
                                opts.LifetimeTelemetry = true;
                                break;

                            default:
                                opts.Message = string.Format("Unknown switch: {0}", str);
                                return opts;
                        }
                    }
                }

                if (nextCapture != null)
                {
                    opts.Message = string.Format("Incomplete command line switch: {0}", lastSwitch);
                }
                else
                {
                    opts.Okay = true;
                }

                return opts;
            }


            public static string Usage()
            {
                return "\nPyramid Technologies Inc (c) 2018\n" +
                       "ptireliance.exe [OPTIONS] [FLAGS]\n\n" +
                       "OPTIONS\n" +
                       "\t-f,--firmware\t\tPath to firmware to flash update with\n" +
                       "\t-c,--config\t\tPath to configuration to apply\n" +
                       "\t-g,--get-config\t\tPath to file current config will be written to\n" +
                       "\t-l,--set-logo\t\tPath to image file to store as logo\n" +
                       "FLAGS\n" +
                       "\t-r,--revision\t\tRead and display printer firmware revision\n" +
                       "\t-pt,--type\t\t\nRead and display printer type" +
                       "\t-s,--status\t\tRead and display printer status\n" +
                       "\t-t,--startup-telem\t\tRead startup telemetry\n" +
                       "\t-T,--lifetime-telem\t\tRead lifetime telemetry\n" +
                       "\t-p,--power\t\tReboot printer immediately\n";
            }
        }
    }
}
