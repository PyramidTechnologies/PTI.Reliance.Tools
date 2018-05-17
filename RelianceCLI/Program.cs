using System;
using PTIRelianceLib;

namespace RelianceCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing HID interop");

            var printer = new ReliancePrinter();
            var revlev = printer.GetRevlev();

            Console.WriteLine("Revision: {0}", revlev);

            Console.ReadKey();
        }
    }
}
