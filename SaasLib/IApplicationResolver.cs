using System;
using Microsoft.AspNetCore.Http;

namespace SaasLib
{
	public interface IApplicationResolver
	{
		bool TryResolveApplication(
			HttpContext httpContext,
			out IApplication application
		);
	}
}