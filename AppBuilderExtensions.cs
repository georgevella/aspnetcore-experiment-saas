using Microsoft.AspNetCore.Builder;

namespace Saas
{
	public static class AppBuilderExtensions
	{
		public static IApplicationBuilder UseSaas(this IApplicationBuilder masterAppBuilder)
		{
			masterAppBuilder.UseMiddleware<SaasMiddleware>();
			return masterAppBuilder;
		}
	}
}