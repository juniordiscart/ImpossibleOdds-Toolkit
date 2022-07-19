namespace ImpossibleOdds.StateMachines
{
	using ImpossibleOdds.Runnables;

	/// <summary>
	/// A state machine that runs through the Runnables system.
	/// </summary>
	/// <typeparam name="TStateKey">The type of the key to identify states with in the state machine.</typeparam>
	public class RunnableStateMachine<TStateKey> : StateMachine<TStateKey>, IRunnable
	{
		/// <inheritdoc />
		void IRunnable.Update()
		{
			Update();
		}
	}
}
