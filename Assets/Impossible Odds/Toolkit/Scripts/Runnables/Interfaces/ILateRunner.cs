namespace ImpossibleOdds.Runnables
{
	public interface ILateRunner
	{
		void Add(ILateRunnable runnable);
		void Remove(ILateRunnable runnable);
	}
}
