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
			services.AddMvc();
		}

		public override void Configure(
			IApplicationBuilder app
		)
		{
			app.UseDeveloperExceptionPage();

			app.UseMvc();
		}
	}
}