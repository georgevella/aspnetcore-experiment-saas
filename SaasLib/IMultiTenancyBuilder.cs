using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SaasLib
{
	public interface IMultiTenancyBuilder<out TApplication>
		where TApplication : class, IApplication
	{		
		IMultiTenancyBuilder<TApplication> UseApplicationResolver<TApplicationReslover>()
			where TApplicationReslover : class, IApplicationResolver;

		IMultiTenancyBuilder<TApplication> For(
			Predicate<TApplication> predicate,
			Action<ITenantConfigurator> configurationCallback
		);

		IMultiTenancyBuilder<TApplication> AddServices(
			Action<IServiceCollection> configureServicesCallback
		);

		IWebHost Build(IApplication application);

		/// <summary>
		/// 	Sets up the default tenant configurator used when no application speecific configurations are registered
		/// or match the current application.
		/// </summary>
		ITenantConfigurator Default { get; }
	}
}