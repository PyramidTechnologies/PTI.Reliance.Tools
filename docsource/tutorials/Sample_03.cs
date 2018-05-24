using System;
using PTIRelianceLib;

namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {
			// Load a firmware file someplace on disk
			var file = BinaryFile.From("my_config.rfg");
			
			// Make sure that file loaded okay
			if (file.Empty)
			{
				Console.WriteLine("Configuration file cannot be read. Does it exist?");
			}
			else
			{

				// Wrap our printer in using so it gets disposed on properly
				using(var printer = new ReliancePrinter())
				{
					
					// Do the config!
					var result = printer.SendConfiguration(file);

					Console.WriteLine("\nConfiguration Update Result: {0}", result);
				}
			}
        }
    }
}
