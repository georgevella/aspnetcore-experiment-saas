using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib.Internal
{
	internal class TenantPipelineBuilder<TApplication> : ITenantPipelineBuilder
		where TApplication : class, IApplication
	{
		private readonly IMultiTenancyBuilder<TApplication> _tenantConfigurator;		

		public TenantPipelineBuilder(IMultiTenancyBuilder<TApplication> tenantConfigurator)
		{
			_tenantConfigurator = tenantConfigurator;
		}
		
		public TenantPipeline BuildTenancyChain(IApplication application)
		{
			var webHost = _tenantConfigurator.Build(application);
			
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