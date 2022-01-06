namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Support objects that need to hook into the FixedUpdate loop.
	/// </summary>
	public interface IFixedRunner
	{
		/// <summary>
		/// Add runnable that wishes to hook into the FixedUpdate loop.
		/// </summary>
		/// <param name="runnable">Runnable to register.</param>
		void AddFixedUpdate(IFixedRunnable runnable);
		/// <summary>
		/// Remove runnable that wishes to unhook from the FixedUpdate loop.
		/// </summary>
		/// <param name="runnable">Runnable to remove.</param>
		void RemoveFixedUpdate(IFixedRunnable runnable);
	}
}
