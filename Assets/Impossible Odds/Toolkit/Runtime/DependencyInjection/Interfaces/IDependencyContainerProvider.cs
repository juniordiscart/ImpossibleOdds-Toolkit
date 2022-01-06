namespace ImpossibleOdds.DependencyInjection
{
	public interface IDependencyContainerProvider
	{
		int Priority
		{
			get;
		}

		IDependencyContainer GetContainer();
	}
}
