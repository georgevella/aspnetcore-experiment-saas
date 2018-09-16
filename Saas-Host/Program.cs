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
                .UseMultiTenancy(
                    builder => builder
                        .UseStartup<TenantStartup>()
                        .UseApplicationResolver<ApplicationResolver>()
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
                    application = new Application() {Id = "Default-Localhost"};
                    break;
                
                case "tenant2.localhost":
                    application = new Application() {Id = "Tenant2"};
                    break;
            }

            return application != null;
        }
    }
    
    public class Application : IApplication
    {
        public string Id { get; set; }
    }
}
