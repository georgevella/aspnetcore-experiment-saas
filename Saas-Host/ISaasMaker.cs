using Microsoft.AspNetCore.Http;

namespace Saas
{
	public interface ISaasMaker
	{
		RequestDelegate BuildTenancyChain();
	}
}