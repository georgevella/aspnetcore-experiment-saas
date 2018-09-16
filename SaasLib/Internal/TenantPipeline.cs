using System;
using Microsoft.AspNetCore.Http;

namespace SaasLib
{
	public class TenantPipeline
	{
		public TenantPipeline(
			RequestDelegate requestDelegate,
			IServiceProvider services
		)
		{
			RequestDelegate = requestDelegate;
			Services = services;
		}

		public RequestDelegate RequestDelegate { get; }
		
		public IServiceProvider Services { get; }
	}
}