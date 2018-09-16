using System;

namespace SaasLib
{
	class SaasApplicationContextAccessor : ISaasApplicationContextAccessor
	{
		public IApplication GetApplication()
		{
			if (SaasApplicationContextStore.TryGet(out var saasAppContext))
			{
				return saasAppContext.Application;
			}

			throw new InvalidOperationException("Not within a Saas application");
		}
	}
}