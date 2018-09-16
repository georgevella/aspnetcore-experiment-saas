using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaasLib;
using SaasLib.Extensions;

namespace Saas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(
            string[] args
        ) =>
            WebHost.CreateDefaultBuilder(args)
                .UseMultiTenancy<SampleApplication>(
                    builder => builder
                        .UseStartup<TenantStartup>()
                        .UseApplicationResolver<ApplicationResolver>()
                        .For(application => application.Id == "Tenant3",
                             configurator => configurator.UseStartup<Tenant3Startup>())
                );
    }

    public class ApplicationResolver : IApplicationResolver
    {
        public bool TryResolveApplication(
            HttpContext httpContext,
            out IApplication application
        )
        {
            application = null;
            
            switch (httpContext.Request.Host.Host)
            {
                case "localhost":
                    application = new SampleApplication() {Id = "Default-Localhost"};
                    break;
                
                case "tenant2.localhost":
                    application = new SampleApplication() {Id = "Tenant2"};
                    break;
                
                case "tenant3.localhost":
                    application = new SampleApplication() { Id = "Tenant3"};
                    break;
            }

            return application != null;
        }
    }
    
    public class SampleApplication : IApplication
    {
        public string Id { get; set; }
    }
}
