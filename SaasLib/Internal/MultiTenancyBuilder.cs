using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib
{
	class MultiTenancyBuilder : IMultiTenancyBuilder
	{
		private readonly IWebHostBuilder _masterWebHostBuilder;
		private List<Action<IWebHostBuilder>> _tenantWebHostBuilderConfigurators = new List<Action<IWebHostBuilder>>();

		public MultiTenancyBuilder(
			IWebHostBuilder masterWebHostBuilder
		)
		{
			_masterWebHostBuilder = masterWebHostBuilder;
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
		
		public IMultiTenancyBuilder UseStartup<TStartup>() where TStartup : class
		{
			_tenantWebHostBuilderConfigurators.Add( builder => builder.UseStartup<TStartup>() );
			return this;
		}

		public IMultiTenancyBuilder UseApplicationResolver<TApplicationResolver>() where TApplicationResolver : class, IApplicationResolver
		{
			_masterWebHostBuilder.ConfigureServices(collection => collection.AddSingleton<IApplicationResolver, TApplicationResolver>());
			return this;
		}
	}
}