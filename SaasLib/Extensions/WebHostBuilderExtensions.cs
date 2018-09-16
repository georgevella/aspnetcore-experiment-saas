using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SaasLib.Internal;

namespace SaasLib.Extensions
{
	public static class WebHostBuilderExtensions
	{
		public static IWebHostBuilder UseMultiTenancy<TApplication>(
			this IWebHostBuilder masterWebHostBuilder,
			Action<IMultiTenancyBuilder<TApplication>> configurator
		)
			where TApplication : class, IApplication
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator));
			}
			
			var builder = new MultiTenancyBuilder<TApplication>(masterWebHostBuilder);
			configurator(builder);

			masterWebHostBuilder.ConfigureServices(
				(
					context,
					collection
				) =>
				{
					collection.AddSingleton<IMultiTenancyBuilder<TApplication>>(builder);
					collection.AddSingleton<ITenantPipelineBuilder, TenantPipelineBuilder<TApplication>>();
				});
			masterWebHostBuilder.UseStartup<MasterStartup>();
			
			return masterWebHostBuilder;
		}
	}
}