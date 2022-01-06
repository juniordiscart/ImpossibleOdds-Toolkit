namespace ImpossibleOdds.DependencyInjection
{
	public class DependencyScope : IDependencyScope
	{
		private IDependencyContainer container = null;

		public IDependencyContainer DependencyContainer
		{
			get { return container; }
		}
	}
}
