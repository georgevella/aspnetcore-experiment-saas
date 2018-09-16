using SaasLib.Internal;

namespace SaasLib
{
	internal interface ITenantPipelineBuilder
	{
		TenantPipeline BuildTenancyChain(IApplication application);
	}
}