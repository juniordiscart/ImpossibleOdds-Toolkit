namespace ImpossibleOdds.Examples.StateMachines
{
	using UnityEngine;
	using UnityEngine.UI;
	using ImpossibleOdds;
	using ImpossibleOdds.StateMachines;

	public abstract class TurnstileState : MonoBehaviour, IState
	{
		[SerializeField]
		private Turnstile.StateKey stateKey = Turnstile.StateKey.None;
		[SerializeField]
		private Outline stateOutline = null;

		private bool isStateActive = false;

		public Turnstile.StateKey StateKey
		{
			get { return stateKey; }
		}

		public bool IsStateActive
		{
			get { return isStateActive; }
		}

		private void Awake()
		{
			stateOutline.enabled = false;
		}

		public virtual void Enter()
		{
			stateOutline.enabled = true;
			isStateActive = true;
			Log.Info("Entered state {0}.", stateKey.DisplayName());
		}

		public virtual void Exit()
		{
			stateOutline.enabled = false;
			isStateActive = false;
			Log.Info("Left state {0}.", stateKey.DisplayName());
		}

		void IState.Update()
		{ }
	}
}
