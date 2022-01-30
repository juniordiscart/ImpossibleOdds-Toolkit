namespace ImpossibleOdds.StateMachines
{
	using System;

	/// <summary>
	/// Base interface for defining a transition between states in a state machine.
	/// </summary>
	public interface IStateTransition
	{
		event Action<IStateTransition> onTriggered;

		/// <summary>
		/// The origin state of this transition.
		/// </summary>
		object From
		{
			get;
		}

		/// <summary>
		/// The destination state of this transition.
		/// </summary>
		object To
		{
			get;
		}

		/// <summary>
		/// Is the condition met to trigger this transition?
		/// </summary>
		bool CanTrigger
		{
			get;
		}

		/// <summary>
		/// Trigger this transition to change the state machine's state.
		/// </summary>
		void Trigger();
	}

	/// <summary>
	/// Base interface for defining a transition between states in a state machine.
	/// </summary>
	public interface IStateTransition<TStateKey> : IStateTransition
	{
		/// <summary>
		/// The origin state of this transition.
		/// </summary>
		new TStateKey From
		{
			get;
		}

		/// <summary>
		/// The destination state of this transition.
		/// </summary>
		new TStateKey To
		{
			get;
		}
	}
}
