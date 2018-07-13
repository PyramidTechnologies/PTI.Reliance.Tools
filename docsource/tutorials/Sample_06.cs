using System;
using System.Collections.Generic;
using System.Threading;
using PTIRelianceLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
	
namespace reliance_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Wrap our printer in using so it gets disposed on properly
            using (var printer = new ReliancePrinter())
            {
				var tel1 = printer.GetPowerupTelemetry();
                var str = JsonConvert.SerializeObject(tel1, Formatting.Indented, new StringEnumConverter());
                Console.WriteLine("Powerup Telementry:\n{0}", str);
				
				var tel2 = printer.GetLifetimeTelemetry();
				var str = JsonConvert.SerializeObject(tel2, Formatting.Indented, new StringEnumConverter());
				Console.WriteLine("Lifetime Telementry:\n{0}", str);
            }
        }
    }
}