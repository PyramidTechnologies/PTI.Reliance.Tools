using System;
using System.Collections.Generic;
using System.Threading;
using PTIRelianceLib;
using PTIRelianceLib.Imaging;

namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Wrap our printer in using so it gets disposed on properly
            using (var printer = new ReliancePrinter())
            {
                var logos = new List<BinaryFile>
                {
                    BinaryFile.From("index.jpg"),
                    BinaryFile.From("index2.jpg"),
                };

                var config = LogoStorageConfig.Default;
                config.Algorithm = DitherAlgorithms.Atkinson;

                var result = printer.StoreLogos(logos, new DevNullMonitor(), config);
                Console.WriteLine("Write Result: {0}", result);

                // use logo index to recall and print
                for(int i = 0 ; i < logos.Count; ++i)
                {
                    printer.PrintLogo(i);

                    // Give ~3 seconds to print
                    Thread.Sleep(3 * 1000);
                }
            }
        }
    }
}