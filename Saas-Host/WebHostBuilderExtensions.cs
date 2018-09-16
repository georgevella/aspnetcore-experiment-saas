using Microsoft.AspNetCore.Hosting;

namespace Saas
{
	public static class WebHostBuilderExtensions
	{
		public static IWebHostBuilder UseMultiTenantStartup<TStartup>(this IWebHostBuilder webHostBuilder) where TStartup : class
		{
			webHostBuilder.UseStartup<MasterStartup<TStartup>>();			
			return webHostBuilder;
		}
	}
}