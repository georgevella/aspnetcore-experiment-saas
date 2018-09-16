using System;
using Microsoft.AspNetCore.Hosting;

namespace SaasLib
{
	public interface IMultiTenancyBuilder<TApplication>
		where TApplication : class, IApplication
	{
		IMultiTenancyBuilder<TApplication> UseStartup<TStartup>()
			where TStartup : class;
	
		IMultiTenancyBuilder<TApplication> UseApplicationResolver<TApplicationReslover>()
			where TApplicationReslover : class, IApplicationResolver;

		IMultiTenancyBuilder<TApplication> For(
			Predicate<TApplication> predicate,
			Action<ITenantConfigurator> configurationCallback
		);

		IWebHost Build(IApplication application);
	}
}