namespace ImpossibleOdds.Examples.StateMachines
{
	using UnityEngine;
	using UnityEngine.UI;
	using ImpossibleOdds.StateMachines;
	using ImpossibleOdds.Runnables;

	/// <summary>
	/// A turnstile implementation based on a state machine.
	/// </summary>
	public class Turnstile : MonoBehaviour
	{
		[SerializeField]
		private TurnstileStateLocked stateLocked = null;
		[SerializeField]
		private TurnstileStateUnlocked stateUnlocked = null;

		[SerializeField]
		private Button btnInsertCoin = null;
		[SerializeField]
		private Button btnTurnHandle = null;

		private RunnableStateMachine<StateKey> stateMachine = null;

		private void Start()
		{
			btnInsertCoin.onClick.AddListener(stateLocked.InsertCoin);
			btnTurnHandle.onClick.AddListener(stateUnlocked.TurnHandle);

			stateMachine = new RunnableStateMachine<StateKey>();

			// Set up the states.
			stateMachine.AddState(stateLocked.StateKey, stateLocked);
			stateMachine.AddState(stateUnlocked.StateKey, stateUnlocked);

			// Add the transitions.
			stateMachine.AddTransition(new TurnstileTransition(StateKey.Locked, StateKey.Unlocked, () => stateLocked.InsertedCoin));
			stateMachine.AddTransition(new TurnstileTransition(StateKey.Unlocked, StateKey.Locked, () => stateUnlocked.HandleTurned));

			// Move the statemachine to its initial state.
			stateMachine.MoveToState(StateKey.Locked);

			// Hook to statemachine to a runner so it will keep monitoring its state.
			SceneRunner.Get.AddUpdate(stateMachine);
		}

		public enum StateKey
		{
			None,
			Locked,
			Unlocked
		}
	}
}
