using System;
using PTIRelianceLib;

namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var printer = new ReliancePrinter())
            {
                var rev = printer.GetFirmwareRevision();
                Console.WriteLine("Firmware revision: {0}", rev);
            }
        }
    }
}
