namespace ImpossibleOdds.StateMachines
{
	/// <summary>
	/// Basic interface for defining a state for a state machine.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Called by the state machine to let the state know it is currently the active state.
		/// </summary>
		void Enter();

		/// <summary>
		/// Called by the state machine to let the state know it should perform an update, if it needs to.
		/// </summary>
		void Update();

		/// <summary>
		/// Called by the state machine to let the state know it is no longer the current active state.
		/// </summary>
		void Exit();
	}
}
