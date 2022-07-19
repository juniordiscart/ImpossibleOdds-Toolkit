namespace ImpossibleOdds.StateMachines
{
	using System;
	using System.Collections.Generic;
	using ImpossibleOdds.Runnables;

	public interface IStateMachine : IRunnable
	{
		event Action<object> onStateChanged;
		event Action<IStateTransition> onTransitionTriggered;

		/// <summary>
		/// The current state the state machine resides in.
		/// </summary>
		object CurrentState
		{
			get;
		}

		/// <summary>
		/// Move the state machine to the state associated with the given key.
		/// </summary>
		/// <param name="stateKey">The key of the state to switch the state machine into.</param>
		void MoveToState(object stateKey);

		/// <summary>
		/// Add a state to the state machine with the given key.
		/// </summary>
		/// <param name="stateKey">The key that will be used to access the state.</param>
		/// <param name="state">The actual state object.</param>
		void AddState(object stateKey, IState state);

		/// <summary>
		/// Add the states to the state machines along with their associated keys.
		/// </summary>
		/// <param name="states">The states to add to the state machine.</param>
		void AddStates(IEnumerable<KeyValuePair<object, IState>> states);

		/// <summary>
		/// Remove the state that's associated with the given key.
		/// </summary>
		/// <param name="stateKey">The key with which the state is associated with.</param>
		void RemoveState(object stateKey);

		/// <summary>
		/// Remove the states associated with the given keys.
		/// </summary>
		/// <param name="stateKeys">The state keys of the states to be removed from the state machine.</param>
		void RemoveStates(IEnumerable<object> stateKeys);

		/// <summary>
		/// Checks whether the state exists for the given key.
		/// </summary>
		/// <param name="stateKey">The key for which to check whether a state exists.</param>
		/// <returns>True, if a state exists for the given key. False otherwise.</returns>
		bool StateExists(object stateKey);

		/// <summary>
		/// Retrieve the state for the given key.
		/// </summary>
		/// <param name="stateKey">The key for which to retrieve the state.</param>
		/// <returns>The state associated with the key, if any.</returns>
		IState GetState(object stateKey);

		/// <summary>
		/// Add a transition between two states.
		/// </summary>
		/// <param name="transition"></param>
		void AddTransition(IStateTransition transition);

		/// <summary>
		/// Add transitions to the state machine.
		/// </summary>
		/// <param name="transitions">The transitions to add to state machine.</param>
		void AddTransitions(IEnumerable<IStateTransition> transitions);

		/// <summary>
		/// Remove the transition from the state machine.
		/// </summary>
		/// <param name="transition">The transition to remove from the state machine.</param>
		void RemoveTransition(IStateTransition transition);
	}

	public interface IStateMachine<TStateKey> : IStateMachine
	{
		new event Action<TStateKey> onStateChanged;
		new event Action<IStateTransition<TStateKey>> onTransitionTriggered;

		new TStateKey CurrentState
		{
			get;
		}

		/// <summary>
		/// Move the state machine to the state associated with the given key.
		/// </summary>
		/// <param name="stateKey">The key of the state to switch the state machine into.</param>
		void MoveToState(TStateKey stateKey);

		/// <summary>
		/// Add a state to the state machine with the given key.
		/// </summary>
		/// <param name="stateKey">The key that will be used to access the state.</param>
		/// <param name="state">The actual state object.</param>
		void AddState(TStateKey stateKey, IState state);

		/// <summary>
		/// Add the states to the state machines along with their associated keys.
		/// </summary>
		/// <param name="states">The states to add to the state machine.</param>
		void AddStates(IEnumerable<KeyValuePair<TStateKey, IState>> states);

		/// <summary>
		/// Remove the state that's associated with the given key.
		/// </summary>
		/// <param name="stateKey">The key with which the state is associated with.</param>
		void RemoveState(TStateKey stateKey);

		/// <summary>
		/// Remove the states associated with the given keys.
		/// </summary>
		/// <param name="stateKeys">The state keys of the states to be removed from the state machine.</param>
		void RemoveStates(IEnumerable<TStateKey> stateKeys);

		/// <summary>
		/// Checks whether the state exists for the given key.
		/// </summary>
		/// <param name="stateKey">The key for which to check whether a state exists.</param>
		/// <returns>True, if a state exists for the given key. False otherwise.</returns>
		bool StateExists(TStateKey stateKey);

		/// <summary>
		/// Retrieve the state for the given key.
		/// </summary>
		/// <param name="stateKey">The key for which to retrieve the state.</param>
		/// <returns>The state associated with the key, if any.</returns>
		IState GetState(TStateKey stateKey);

		/// <summary>
		/// Add a transition between two states.
		/// </summary>
		/// <param name="transition">The transition to add to the state machine.</param>
		void AddTransition(IStateTransition<TStateKey> transition);

		/// <summary>
		/// Add transitions to the state machine.
		/// </summary>
		/// <param name="transitions">The transitions to add to state machine.</param>
		void AddTransitions(IEnumerable<IStateTransition<TStateKey>> transitions);

		/// <summary>
		/// Remove the transition from the state machine.
		/// </summary>
		/// <param name="transition">The transition to remove from the state machine.</param>
		void RemoveTransition(IStateTransition<TStateKey> transition);
	}
}
