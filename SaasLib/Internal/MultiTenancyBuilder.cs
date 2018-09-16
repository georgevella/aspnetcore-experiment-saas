using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib.Internal
{
	class MultiTenancyBuilder<TApplication> : IMultiTenancyBuilder<TApplication> where TApplication : class, IApplication
	{
		private readonly IWebHostBuilder _masterWebHostBuilder;
		private readonly TenantConfigurator _defaultTenantConfigurator = new TenantConfigurator();
		private readonly List<KeyValuePair<Predicate<TApplication>, ITenantConfigurator>> _filteredConfigurators 
			= new List<KeyValuePair<Predicate<TApplication>, ITenantConfigurator>>();

		public MultiTenancyBuilder(
			IWebHostBuilder masterWebHostBuilder
		)
		{
			_masterWebHostBuilder = masterWebHostBuilder;
		}

		public IMultiTenancyBuilder<TApplication> For(
			Predicate<TApplication> predicate,
			Action<ITenantConfigurator> configurationCallback
		)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			if (configurationCallback == null)
			{
				throw new ArgumentNullException(nameof(configurationCallback));
			}

			var tc = new TenantConfigurator();
			configurationCallback(tc);
			_filteredConfigurators.Add(new KeyValuePair<Predicate<TApplication>, ITenantConfigurator>(predicate, tc));
			
			return this;
		}

		public IWebHost Build(IApplication application)
		{
			var app = application as TApplication;

			if (_filteredConfigurators.Any(x => x.Key(app)))
			{
				var configurator = _filteredConfigurators.First(x => x.Key(app));
				return configurator.Value.Build();
			}

			return _defaultTenantConfigurator.Build();
		}
		
		public IMultiTenancyBuilder<TApplication> UseStartup<TStartup>() where TStartup : class
		{
			_defaultTenantConfigurator.UseStartup<TStartup>();
			return this;
		}

		public IMultiTenancyBuilder<TApplication> UseApplicationResolver<TApplicationResolver>() where TApplicationResolver : class, IApplicationResolver
		{
			_masterWebHostBuilder.ConfigureServices(collection => collection.AddSingleton<IApplicationResolver, TApplicationResolver>());
			return this;
		}
	}
}