namespace ImpossibleOdds.Runnables
{
	using System.Collections;

	public interface IFixedRunner
	{
		void Add(IFixedRunnable runnable);
		void Remove(IFixedRunnable runnable);
		void RunRoutine(IEnumerator routineHandle);
	}
}
