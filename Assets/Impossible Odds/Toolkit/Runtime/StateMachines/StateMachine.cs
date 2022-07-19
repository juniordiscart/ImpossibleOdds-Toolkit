namespace ImpossibleOdds.StateMachines
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using ImpossibleOdds.Runnables;

	public class StateMachine<TStateKey> : IStateMachine<TStateKey>
	{
		private ConcurrentDictionary<TStateKey, IState> stateMapping = new ConcurrentDictionary<TStateKey, IState>();
		private ConcurrentDictionary<TStateKey, List<IStateTransition<TStateKey>>> transitions = new ConcurrentDictionary<TStateKey, List<IStateTransition<TStateKey>>>();
		private TStateKey currentState;
		private bool initialised = false;

		public event Action<TStateKey> onStateChanged;
		public event Action<IStateTransition<TStateKey>> onTransitionTriggered;

		/// <inheritdoc />
		public TStateKey CurrentState
		{
			get => currentState;
		}

		/// <inheritdoc />
		object IStateMachine.CurrentState
		{
			get => CurrentState;
		}

		/// <inheritdoc />
		private IState CurrentStateObj
		{
			get => stateMapping.TryGetValue(currentState, out IState state) ? state : null;
		}

		/// <inheritdoc />
		event Action<object> IStateMachine.onStateChanged
		{
			add
			{
				if (value is Action<TStateKey> typedValue)
				{
					onStateChanged += typedValue;
				}
				else
				{
					throw new NotSupportedException(value.GetType().Name);
				}
			}
			remove
			{
				if (value is Action<TStateKey> typedValue)
				{
					onStateChanged -= typedValue;
				}
				else
				{
					throw new NotSupportedException(value.GetType().Name);
				}
			}
		}

		/// <inheritdoc />
		event Action<IStateTransition> IStateMachine.onTransitionTriggered
		{
			add
			{
				if (value is Action<IStateTransition<TStateKey>> typedValue)
				{
					onTransitionTriggered += typedValue;
				}
				else
				{
					throw new NotSupportedException(value.GetType().Name);
				}
			}
			remove
			{
				if (value is Action<IStateTransition<TStateKey>> typedValue)
				{
					onTransitionTriggered -= typedValue;
				}
				else
				{
					throw new NotSupportedException(value.GetType().Name);
				}
			}
		}

		/// <inheritdoc />
		public void AddState(TStateKey stateKey, IState state)
		{
			stateKey.ThrowIfNull(nameof(stateKey));
			state.ThrowIfNull(nameof(state));
			stateMapping.AddOrUpdate(stateKey, state, (sk, s) => state);
		}

		/// <inheritdoc />
		void IStateMachine.AddState(object stateKey, IState state)
		{
			if (stateKey is TStateKey typedStateKey)
			{
				AddState(typedStateKey, state);
			}
			else
			{
				throw new NotSupportedException(stateKey.GetType().Name);
			}
		}

		/// <inheritdoc />
		public void MoveToState(TStateKey stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));

			if (!StateExists(stateKey))
			{
				throw new KeyNotFoundException(nameof(stateKey));
			}

			// If the state machine is already in this state, then don't bother.
			if (initialised && currentState.Equals(stateKey))
			{
				return;
			}

			// Moving to a state always initializes the state machine.
			initialised = true;

			if (CurrentStateObj != null)
			{
				CurrentStateObj.Exit();
			}

			currentState = stateKey;
			if (CurrentStateObj != null)
			{
				CurrentStateObj.Enter();
			}

			onStateChanged.InvokeIfNotNull(stateKey);
		}

		/// <inheritdoc />
		void IStateMachine.MoveToState(object stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));

			if (stateKey is TStateKey typedStateKey)
			{
				MoveToState(typedStateKey);
			}
			else
			{
				throw new NotSupportedException(stateKey.GetType().Name);
			}
		}

		/// <inheritdoc />
		public void RemoveState(TStateKey stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));

			// If the current state is exactly this state, then perform the exit operation.
			if (currentState.Equals(stateKey) && (CurrentStateObj != null))
			{
				CurrentStateObj.Exit();
			}

			stateMapping.TryRemove(stateKey, out _);

			lock (transitions)
			{
				transitions.TryRemove(stateKey, out _);
				foreach (TStateKey sk in transitions.Keys)
				{
					transitions[sk].RemoveAll(t => t.To.Equals(stateKey));
				}
			}
		}

		/// <inheritdoc />
		void IStateMachine.RemoveState(object stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));
			if (stateKey is TStateKey typedStateKey)
			{
				RemoveState(typedStateKey);
			}
			else
			{
				throw new NotSupportedException(stateKey.GetType().Name);
			}
		}

		/// <inheritdoc />
		public bool StateExists(TStateKey stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));
			return stateMapping.ContainsKey(stateKey);
		}

		/// <inheritdoc />
		bool IStateMachine.StateExists(object stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));

			if (stateKey is TStateKey typedStateKey)
			{
				return StateExists(typedStateKey);
			}
			else
			{
				throw new NotSupportedException(stateKey.GetType().Name);
			}
		}

		/// <inheritdoc />
		public IState GetState(TStateKey stateKey)
		{
			stateKey.ThrowIfNull(nameof(stateKey));
			return stateMapping.TryGetValue(stateKey, out IState state) ? state : null;
		}

		/// <inheritdoc />
		IState IStateMachine.GetState(object stateKey)
		{
			if (stateKey is TStateKey typedStateKey)
			{
				return GetState(typedStateKey);
			}
			else
			{
				throw new NotSupportedException(stateKey.GetType().Name);
			}
		}

		/// <inheritdoc />
		public void AddStates(IEnumerable<KeyValuePair<TStateKey, IState>> states)
		{
			states.ThrowIfNull(nameof(states));
			foreach (KeyValuePair<TStateKey, IState> state in states)
			{
				AddState(state.Key, state.Value);
			}
		}

		/// <inheritdoc />
		public void RemoveStates(IEnumerable<TStateKey> stateKeys)
		{
			stateKeys.ThrowIfNull(nameof(stateKeys));
			foreach (TStateKey stateKey in stateKeys)
			{
				RemoveState(stateKey);
			}
		}

		/// <inheritdoc />
		public void AddTransition(IStateTransition<TStateKey> transition)
		{
			transition.ThrowIfNull(nameof(transition));

			if (transition.From.Equals(transition.To))
			{
				throw new StateMachineException("The transition's origin state is the same as its destination state ({0}).", transition.From.ToString());
			}
			else if (!StateExists(transition.From))
			{
				throw new StateMachineException("The transition's origin state ({0}) is not present in this state machine.", transition.From.ToString());
			}
			else if (!StateExists(transition.To))
			{
				throw new StateMachineException("The transition's destination state ({0}) is not present in this state machine.", transition.To.ToString());
			}

			List<IStateTransition<TStateKey>> stateTransitions = transitions.GetOrAdd(transition.From, (s) => new List<IStateTransition<TStateKey>>());
			lock (stateTransitions)
			{
				if (!transitions[transition.From].Contains(transition))
				{
					transitions[transition.From].Add(transition);
					transition.onTriggered += OnTransitionTriggered;
				}
			}
		}

		/// <inheritdoc />
		public void AddTransitions(IEnumerable<IStateTransition<TStateKey>> transitions)
		{
			transitions.ThrowIfNull(nameof(transitions));
			foreach (IStateTransition<TStateKey> transition in transitions)
			{
				AddTransition(transition);
			}
		}

		/// <inheritdoc />
		public void RemoveTransition(IStateTransition<TStateKey> transition)
		{
			transition.ThrowIfNull(nameof(transition));

			if (transitions.TryGetValue(transition.From, out List<IStateTransition<TStateKey>> stateTransitions))
			{
				lock (stateTransitions)
				{
					stateTransitions.Remove(transition);
					transition.onTriggered -= OnTransitionTriggered;
				}
			}
		}

		/// <inheritdoc />
		void IStateMachine.AddStates(IEnumerable<KeyValuePair<object, IState>> states)
		{
			states.ThrowIfNull(nameof(states));
			if (states is IEnumerable<KeyValuePair<TStateKey, IState>> typedStates)
			{
				AddStates(typedStates);
			}
			else
			{
				throw new NotSupportedException(states.GetType().Name);
			}
		}

		/// <inheritdoc />
		void IStateMachine.RemoveStates(IEnumerable<object> stateKeys)
		{
			stateKeys.ThrowIfNull(nameof(stateKeys));
			if (stateKeys is IEnumerable<TStateKey> typedStateKeys)
			{
				RemoveStates(typedStateKeys);
			}
			else
			{
				throw new NotSupportedException(stateKeys.GetType().Name);
			}
		}

		/// <inheritdoc />
		void IStateMachine.AddTransition(IStateTransition transition)
		{
			transition.ThrowIfNull(nameof(transition));

			if (transition is IStateTransition<TStateKey> typedTransition)
			{
				AddTransition(typedTransition);
			}
			else
			{
				throw new NotSupportedException(transition.GetType().Name);
			}
		}

		/// <inheritdoc />
		void IStateMachine.AddTransitions(IEnumerable<IStateTransition> transitions)
		{
			transitions.ThrowIfNull(nameof(transitions));

			if (transitions is IEnumerable<IStateTransition<TStateKey>> typedTransitions)
			{
				AddTransitions(typedTransitions);
			}
			else
			{
				throw new NotSupportedException(transitions.GetType().Name);
			}
		}

		/// <inheritdoc />
		void IStateMachine.RemoveTransition(IStateTransition transition)
		{
			transition.ThrowIfNull(nameof(transition));

			if (transition is IStateTransition<TStateKey> typedTransition)
			{
				RemoveTransition(typedTransition);
			}
			else
			{
				throw new NotSupportedException(transition.GetType().Name);
			}
		}

		/// <summary>
		/// Check the current state for any outgoing transitions that can be triggered.
		/// </summary>
		public void Update()
		{
			if (!initialised)
			{
				return;
			}

			IState currentState = CurrentStateObj;
			if (currentState != null)
			{
				currentState.Update();

				// Check whether any transition originating from the current state is able to trigger.
				if (transitions.TryGetValue(CurrentState, out List<IStateTransition<TStateKey>> stateTransitions))
				{
					lock (stateTransitions)
					{
						foreach (IStateTransition<TStateKey> transition in stateTransitions)
						{
							if (transition.CanTrigger)
							{
								transition.Trigger();
								break;
							}
						}
					}
				}
			}
		}

		private void OnTransitionTriggered(IStateTransition transition)
		{
			if (transition is IStateTransition<TStateKey> typedTransition)
			{
				if (currentState.Equals(typedTransition.From))
				{
					onTransitionTriggered.InvokeIfNotNull(typedTransition);
					MoveToState(typedTransition.To);
				}
				else
				{
					Log.Warning("A transition between states {0} and {1} has triggered, but the state machine is not in state {0}. The state machine will not move to state {1}.", transition.From.ToString(), transition.To.ToString());
				}
			}
		}
	}
}
