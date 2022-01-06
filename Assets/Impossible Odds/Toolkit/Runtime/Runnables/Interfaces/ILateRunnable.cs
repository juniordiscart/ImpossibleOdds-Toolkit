namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Interface for objects interested in hooking into the LateUpdate-loop.
	/// </summary>
	public interface ILateRunnable
	{
		void LateUpdate();
	}
}
