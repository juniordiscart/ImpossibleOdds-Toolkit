namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Interface for objects interested in hooking into the FixedUpdate-loop.
	/// </summary>
	public interface IFixedRunnable
	{
		void FixedUpdate();
	}
}
