using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib
{
	internal class TenantPipelineBuilder : ITenantPipelineBuilder
	{
		private readonly IMultiTenancyBuilder _multiTenancyBuilder;		

		public TenantPipelineBuilder(IMultiTenancyBuilder multiTenancyBuilder)
		{
			_multiTenancyBuilder = multiTenancyBuilder;
		}
		
		public TenantPipeline BuildTenancyChain(IApplication application)
		{
			var webHost = _multiTenancyBuilder.Build();
			
			var serviceProvider = webHost.Services;
			var serverFeatures = webHost.ServerFeatures;

			var appBuilderFactory = serviceProvider.GetRequiredService<IApplicationBuilderFactory>();
			var appBuilder = appBuilderFactory.CreateBuilder(serverFeatures);
			var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			appBuilder.Use(async (context, next) =>
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

			configure(appBuilder);
			
			var branchDelegate = appBuilder.Build();
			return new TenantPipeline(branchDelegate, serviceProvider);
		}
	}
}