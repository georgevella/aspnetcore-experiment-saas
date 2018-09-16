using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib.Internal
{
	class TenantConfigurator : ITenantConfigurator
	{
		private readonly List<Action<IWebHostBuilder>> _tenantWebHostBuilderConfigurators = new List<Action<IWebHostBuilder>>();
		
		public ITenantConfigurator UseStartup<TStartup>() where TStartup : class
		{
			_tenantWebHostBuilderConfigurators.Add( builder => builder.UseStartup<TStartup>() );
			return this;
		}

		public IWebHost Build()
		{
			var _tenantWebHostBuilder = new WebHostBuilder();
			_tenantWebHostBuilder.ConfigureServices(
				collection =>
					collection
						.AddScoped<ISaasApplicationContextAccessor, SaasApplicationContextAccessor>()
			);
			
			_tenantWebHostBuilderConfigurators.ForEach( a => a(_tenantWebHostBuilder) );
			
			return _tenantWebHostBuilder.UseKestrel()
				.Build();
		}
	}
}