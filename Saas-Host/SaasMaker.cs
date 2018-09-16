using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Saas
{
	public class SaasMaker<PerTenantStartup> : ISaasMaker
		where PerTenantStartup : class
	{
		private readonly IApplicationBuilder _appBuilder;

		public SaasMaker()
		{
			var webHost = new WebHostBuilder().UseKestrel().UseStartup<PerTenantStartup>().Build();
			var serviceProvider = webHost.Services;
			var serverFeatures = webHost.ServerFeatures;

			var appBuilderFactory = serviceProvider.GetRequiredService<IApplicationBuilderFactory>();
			_appBuilder = appBuilderFactory.CreateBuilder(serverFeatures);
			var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			_appBuilder.Use(async (context, next) =>
			{
				using (var scope = factory.CreateScope())
				{
					context.RequestServices = scope.ServiceProvider;
					await next();
				}
			});

			var startup = serviceProvider.GetService<IStartup>();
			var startupFilters = serviceProvider.GetServices<IStartupFilter>();
            
			Action<IApplicationBuilder> configure = startup.Configure;
			foreach (var filter in startupFilters.Reverse())
			{
				configure = filter.Configure(configure);
			}

			configure(_appBuilder);
		}
		public RequestDelegate BuildTenancyChain()
		{
			var branchDelegate = _appBuilder.Build();
			return branchDelegate;
		}
	}
}