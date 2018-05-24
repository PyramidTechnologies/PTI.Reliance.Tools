using System;
using PTIRelianceLib;

namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {
			// Load a firmware file someplace on disk
			var file = BinaryFile.From("reliance_1.27.171.ptix");
			
			// Make sure that file loaded okay
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
					Console.Write("{0:0.00}%", o.Progress*100);
				};
				
				// Wrap our printer in using so it gets disposed on properly
				using(var printer = new ReliancePrinter())
				{
					// Do the update!
					var result = printer.FlashUpdateTarget(file, monitor);

					Console.WriteLine("\nFlash Update Result: {0}", result);
				}
			}
        }
    }
}
