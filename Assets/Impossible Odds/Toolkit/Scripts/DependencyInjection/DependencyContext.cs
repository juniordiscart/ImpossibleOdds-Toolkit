namespace ImpossibleOdds.DependencyInjection
{
	public class DependencyContext : IDependencyContext
	{
		private IDependencyContainer container = null;

		public IDependencyContainer DependencyContainer
		{
			get { return container; }
		}
	}
}
