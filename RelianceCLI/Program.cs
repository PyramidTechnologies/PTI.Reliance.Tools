using System;
using System.Collections.Generic;
using PTIRelianceLib;

namespace RelianceCLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var opts = Options.Parse(args);

            try
            {
                Run(opts);
            }
            catch (PTIException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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

                var revlev = printer.GetFirmwareRevision();
                Console.WriteLine("Revision: {0}", revlev);

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
                        var monitor = new ConsoleProgressBar();
                        var result = printer.FlashUpdateTarget(file, monitor);

                        Console.WriteLine("Result: {0}", result);
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
                    }
                }
            }
        }

        private struct Options
        {
            public string FirmwareFilePath;

            public string ConfigFilePath;

            public static Options Parse(IEnumerable<string> args)
            {
                var opts = new Options();

                Action<string> nextCapture = null;

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
                            case "-f":
                            case "--firmware":
                                nextCapture = s => opts.FirmwareFilePath = s;
                                break;

                            case "-c":
                            case "--configuration":
                                nextCapture = s => opts.ConfigFilePath = s;
                                break;

                            default:
                                Console.WriteLine("Unknown switch: {0}", str);
                                return new Options();
                        }
                    }
                }

                return opts;
            }
        }
    }
}
