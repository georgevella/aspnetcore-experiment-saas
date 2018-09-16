using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SaasLib.Internal;

namespace SaasLib.Extensions
{
	public static class WebHostBuilderExtensions
	{
		public static IWebHostBuilder UseMultiTenancy(
			this IWebHostBuilder masterWebHostBuilder,
			Action<IMultiTenancyBuilder> configurator
		)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator));
			}
			
			var builder = new MultiTenancyBuilder(masterWebHostBuilder);
			configurator(builder);

			masterWebHostBuilder.ConfigureServices(
				(
					context,
					collection
				) =>
				{
					collection.AddSingleton<IMultiTenancyBuilder>(builder);
					collection.AddSingleton<ITenantPipelineBuilder, TenantPipelineBuilder>();
				});
			masterWebHostBuilder.UseStartup<MasterStartup>();
			
			return masterWebHostBuilder;
		}
	}
}