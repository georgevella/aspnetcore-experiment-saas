using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Saas
{
	public class TenantStartup : StartupBase
	{
		public override void ConfigureServices(
			IServiceCollection services
		)
		{
            
		}

		public override void Configure(
			IApplicationBuilder app
		)
		{
			app.Use(async (
						context,
						func
					) =>
					{
						await context.Response.WriteAsync("Hello world");
					});
		}
	}
}