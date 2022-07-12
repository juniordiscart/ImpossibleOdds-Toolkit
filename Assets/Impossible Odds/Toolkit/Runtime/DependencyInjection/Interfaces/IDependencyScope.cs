namespace ImpossibleOdds.DependencyInjection
{
	public interface IDependencyScope
	{
		/// <summary>
		/// The resources associated with this scope.
		/// </summary>
		IDependencyContainer DependencyContainer
		{
			get;
		}

		/// <summary>
		/// Inject the objects under this scope with its resources.
		/// </summary>
		void Inject();
	}
}
