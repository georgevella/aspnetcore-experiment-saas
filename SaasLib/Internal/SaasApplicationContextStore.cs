using System.Threading;

namespace SaasLib
{
	internal class SaasApplicationContextStore
	{
		private static readonly AsyncLocal<SaasApplicationContext> AsyncStorage = new AsyncLocal<SaasApplicationContext>();

		public static void Store(
			SaasApplicationContext saasApplicationContext
		)
		{
			AsyncStorage.Value = saasApplicationContext;
		} 
		
		public static bool TryGet(out SaasApplicationContext saasApplicationContext)
		{
			if (AsyncStorage.Value != null)
			{
				saasApplicationContext = AsyncStorage.Value;
				return true;
			}

			saasApplicationContext = null;
			return false;
		}
	}
}