using Microsoft.AspNetCore.Hosting;

namespace SaasLib
{
	public interface ITenantConfigurator
	{
		ITenantConfigurator UseStartup<TStartup>()
			where TStartup : class;

		IWebHost Build();
	}
}