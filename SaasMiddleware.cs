using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Saas
{
	public class SaasMiddleware
	{        
		private readonly ISaasMaker _saasMaker;
		private readonly ILogger<SaasMiddleware> _logger;        
		private readonly ConcurrentDictionary<string, RequestDelegate> _requestDelegateMap = new ConcurrentDictionary<string, RequestDelegate>();
		public SaasMiddleware(
			RequestDelegate next,     
			ISaasMaker saasMaker,
			ILogger<SaasMiddleware> logger
		)
		{            
			_saasMaker = saasMaker;
			_logger = logger;
		}
        
		public async Task Invoke(
			HttpContext context
		)
		{
			// todo: ask application management service to determine application from incoming domain request
            
			// todo: if new application cache built request delegate from saas maker

			var host = context.Request.Host;
			var requestDelegate = _requestDelegateMap.GetOrAdd(
				host.ToString(), 
				(s, builder) => builder.BuildTenancyChain(), 
				_saasMaker
			);

			await requestDelegate(context);
		}

	}
}