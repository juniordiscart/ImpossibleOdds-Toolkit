namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Support objects that need to hook into the Update loop.
	/// </summary>
	public interface IRunner
	{
		/// <summary>
		/// Add runnable that whishes to hook into the Update loop.
		/// </summary>
		/// <param name="runnable">Runnable to register.</param>
		void AddUpdate(IRunnable runnable);
		/// <summary>
		/// Remove runnable that wishes to unhook from the Update loop.
		/// </summary>
		/// <param name="runnable">Runnable to remove.</param>
		void RemoveUpdate(IRunnable runnable);
	}
}
