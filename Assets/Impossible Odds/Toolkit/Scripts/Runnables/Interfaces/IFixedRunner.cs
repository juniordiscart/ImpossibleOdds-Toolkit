namespace ImpossibleOdds.Runnables
{

	public interface IFixedRunner
	{
		void Add(IFixedRunnable runnable);
		void Remove(IFixedRunnable runnable);
	}
}
