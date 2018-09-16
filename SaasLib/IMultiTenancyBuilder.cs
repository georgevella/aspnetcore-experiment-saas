using Microsoft.AspNetCore.Hosting;

namespace SaasLib
{
	public interface IMultiTenancyBuilder
	{
		IMultiTenancyBuilder UseStartup<TStartup>()
			where TStartup : class;

		IMultiTenancyBuilder UseApplicationResolver<TApplicationReslover>()
			where TApplicationReslover : class, IApplicationResolver;

		IWebHost Build();
	}
}