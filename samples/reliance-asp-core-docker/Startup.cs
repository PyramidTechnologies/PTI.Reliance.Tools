using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PTIRelianceLib;

namespace reliance_asp_core_docker
{
    /// <summary>
    /// Due to ASP's stateless nature we need to take special care with our hardware access.
    /// Note, that there are many better was to handle this problem. This is the bare minimum
    /// to produce a working sample.
    /// </summary>
    public static class PrinterService
    {
	private static readonly object _mLock = new object();
        private static ReliancePrinter _mPrinter;	

	static PrinterService() 
	{
	    _mPrinter = new ReliancePrinter();
	}

	public static Revlev GetRevision()
	{
	    lock(_mLock)
	    {
	        return _mPrinter.GetFirmwareRevision();
	    }
	}
    }


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Simply write the revision to test that a) HID is happy and b) device access is happy
            app.Run(async (context) =>
            {                
                await context.Response.WriteAsync(PrinterService.GetRevision().ToString());               
            });
        }
    }
}
