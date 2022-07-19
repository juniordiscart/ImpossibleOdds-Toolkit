namespace ImpossibleOdds.Examples.StateMachines
{
	using System;
	using System.Collections.Generic;
	using ImpossibleOdds.StateMachines;
	using UnityEngine;

	public class TrafficLight : MonoBehaviour
	{
		[SerializeField]
		private TrafficLightState green;
		[SerializeField]
		private TrafficLightState yellow;
		[SerializeField]
		private TrafficLightState red;

		[SerializeField]
		private List<TransitionDescription> transitions;

		private StateMachine<TrafficLightStateKey> stateMachine = null;

		private void Start()
		{
			// Build the state machine.
			stateMachine = new StateMachine<TrafficLightStateKey>();
			stateMachine.AddState(TrafficLightStateKey.Green, green);
			stateMachine.AddState(TrafficLightStateKey.Yellow, yellow);
			stateMachine.AddState(TrafficLightStateKey.Red, red);

			// Add the state machine's transitions.
			foreach (TransitionDescription t in transitions)
			{
				stateMachine.AddTransition(new TrafficLightTransition(t.from.StateKey, t.to.StateKey, () => t.from.TimeActive >= t.time));
			}

			// Move the state machine to its initial state.
			stateMachine.MoveToState(TrafficLightStateKey.Green);
		}

		private void Update()
		{
			stateMachine.Update();
		}

		[Serializable]
		private struct TransitionDescription
		{
			public TrafficLightState from;
			public TrafficLightState to;
			[Min(0f), Range(0f, 60f)]
			public float time;
		}
	}

}
