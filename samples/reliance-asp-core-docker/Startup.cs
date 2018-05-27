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
                using(var printer = new ReliancePrinter())
                {
                    var rev = printer.GetFirmwareRevision();
                    await context.Response.WriteAsync(rev.ToString());
                }
            });
        }
    }
}
