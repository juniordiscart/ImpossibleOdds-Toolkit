namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Support objects that need to hook into the LateUpdate loop.
	/// </summary>
	public interface ILateRunner
	{
		/// <summary>
		/// Add runnable that wishes to hook into the LateUpdate loop.
		/// </summary>
		/// <param name="runnable">Runnable to register.</param>
		void AddLateUpdate(ILateRunnable runnable);
		/// <summary>
		/// Remove runnable that whishes to unhook from the LateUpdate loop.
		/// </summary>
		/// <param name="runnable">Runnable to remove.</param>
		void RemoveLateUpdate(ILateRunnable runnable);
	}
}
