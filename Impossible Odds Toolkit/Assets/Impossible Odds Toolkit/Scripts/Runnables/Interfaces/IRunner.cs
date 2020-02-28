namespace ImpossibleOdds.Runnables
{
	using System.Collections;

	public interface IRunner
	{
		void Add(IRunnable runnable);
		void Remove(IRunnable runnable);
		void RunRoutine(IEnumerator routineHandle);
		void StopRoutine(IEnumerator routineHandle);
	}
}
