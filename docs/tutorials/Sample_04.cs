using System;
using PTIRelianceLib;

namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {

			// Wrap our printer in using so it gets disposed on properly
			using(var printer = new ReliancePrinter())
			{
				// Get the status!
                var status = printer.GetStatus();
				
				// Print the summary in a block of text
                Console.WriteLine("Printer status:\n{0}", status);
				
				// We can also get more specific details
				var jammed = status.PrinterErrors.HasFlag(ErrorStatuses.Jammed);
                Console.WriteLine("Printer Jammed? :{0}", jammed);
				
				var lidOpen = status.PrinterErrors.HasFlag(ErrorStatuses.PlatenOpen);
				Console.WriteLine("Lid Open? :{0}", lidOpen);
				
				var hasPaper = status.SensorStatus.HasFlag(SensorStatuses.Path);
                Console.WriteLine("Has Paper? :{0}", hasPaper);
			}
        }
    }
}
