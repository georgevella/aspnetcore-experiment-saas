using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SaasLib.Internal
{
	internal class SaasMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ITenantPipelineBuilder _tenantPipelineBuilder;
		private readonly IApplicationResolver _applicationResolver;
		private readonly ILogger<SaasMiddleware> _logger;        
		private readonly ConcurrentDictionary<string, TenantPipeline> _requestDelegateMap = new ConcurrentDictionary<string, TenantPipeline>();
		public SaasMiddleware(
			RequestDelegate next,     
			ITenantPipelineBuilder tenantPipelineBuilder,
			IApplicationResolver applicationResolver,
			ILogger<SaasMiddleware> logger
		)
		{
			_next = next;
			_tenantPipelineBuilder = tenantPipelineBuilder;
			_applicationResolver = applicationResolver;
			_logger = logger;
		}
        
		public async Task Invoke(
			HttpContext context
		)
		{
			if (_applicationResolver.TryResolveApplication(context, out var application))
			{
				var pipeline = _requestDelegateMap.GetOrAdd(
					application.Id,
					s => _tenantPipelineBuilder.BuildTenancyChain(application)
				);
				
				var applicationContext = new SaasApplicationContext(application);
				SaasApplicationContextStore.Store(applicationContext);
				
				await pipeline.RequestDelegate(context);
			}
			else
			{
				await _next(context);
			}
		}

	}
}