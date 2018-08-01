using System;
using System.Collections.Generic;
using PTIRelianceLib;

namespace RelianceCLI
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using PTIRelianceLib.Imaging;
    using PTIRelianceLib.Protocol;

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

            Console.WriteLine("Total Reliance printers connected: {0}",
                Library.CountAttachedReliance());

            if (opts.MultiTestCount > 0)
            {
                TestCrossTalk(opts.MultiTestCount, 10000);
                return;
            }

            using (var printer = new ReliancePrinter(opts.FilterDevicePath))
            {
                Console.WriteLine("Connected to device:\n\tPath: {0}", printer.DevicePath);

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

        private static void TestCrossTalk(int count, int limit)
        {
            // Build a list of printers and their serial numbers
            var printers = new List<IPyramidDevice>();
            var serialNumbers = new List<string>();
            for (var i = 0; i < count; ++i)
            {
                var printer = new ReliancePrinter();
                printers.Add(printer);
                serialNumbers.Add(printer.GetSerialNumber());
            }

            // Make sure this is a fair test, no duplicate or invalid serial numbers
            if (serialNumbers.Any(string.IsNullOrEmpty))
            {
                Console.WriteLine("Test aborted, at least one device reported an invalid serial number");
            }
            if (serialNumbers.Count != serialNumbers.Distinct().Count())
            {
                Console.WriteLine("Test aborted, at least two devices reported the same serial number");
            }       

            // Request various pieces of data from device, expectedSn is validated against known true serial number
            void TestLoop(IPyramidDevice device, string expectedSn)
            {
                for (var i = 0; i < limit; ++i)
                {
                    var sn = device.GetSerialNumber();
                    var revlev = device.GetFirmwareRevision();                   
                    if (!expectedSn.Equals(sn))
                    {
                        throw new Exception(string.Format("Printer 1 Failure. Got {0}, expected: {1} ({2})", sn, expectedSn, revlev));
                    }

                    Thread.Yield();
                }
            }

            // Start all threads
            var threads = new List<Thread>();
            for (var i = 0; i < count; ++i)
            {
                var printer = printers[i];
                var sn = serialNumbers[i];
                threads.Add(new Thread(() => TestLoop(printer, sn)));
            }

            var start = DateTime.Now;
            foreach (var t in threads)
            {
                t.Start();
            }
            foreach (var t in threads)
            {
                t.Join();
            }   
            var end = DateTime.Now;

            var delta = (end - start).ToString();
            Console.WriteLine("Ran {0} iterations on each of {1} printers without error in {2}", limit, count, delta);
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

            public bool GetStatus;

            public string SaveConfigPath;

            public bool LifetimeTelemetry;

            public bool StartupTelemetry;

            public string FilterDevicePath;

            public int MultiTestCount;

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

                            case "--devicepath":
                                nextCapture = s => opts.FilterDevicePath = s;
                                break;

                            case "--multitest":
                                nextCapture = s => int.TryParse(s, out opts.MultiTestCount);                               
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
                       "\t--devicepath\t\tUSB device path to connect to" +
                       "\t--multitest\t\tTest N printers in parallel" +
                       "FLAGS\n" +
                       "\t-r,--revision\t\tRead and display printer firmware revision\n" +
                       "\t-s,--status\t\tRead and display printer status\n" +
                       "\t-t,--startup-telem\t\tRead startup telemetry\n" +
                       "\t-T,--lifetime-telem\t\tRead lifetime telemetry\n" +
                       "\t-p,--power\t\tReboot printer immediately\n";
            }
        }
    }
}