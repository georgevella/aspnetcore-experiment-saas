namespace SaasLib
{
	internal class SaasApplicationContext
	{
		public SaasApplicationContext(
			IApplication application
		)
		{
			Application = application;
		}

		public IApplication Application { get; }
	}
}