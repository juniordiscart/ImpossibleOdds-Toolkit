namespace ImpossibleOdds.Examples.StateMachines
{
	using System;
	using ImpossibleOdds;
	using ImpossibleOdds.StateMachines;

	public class TurnstileTransition : IStateTransition<Turnstile.StateKey>
	{
		private Turnstile.StateKey from;
		private Turnstile.StateKey to;
		private Func<bool> triggerEval;

		public event Action<IStateTransition> onTriggered;

		public Turnstile.StateKey From
		{
			get => from;
		}

		public Turnstile.StateKey To
		{
			get => to;
		}

		public bool CanTrigger
		{
			get => triggerEval();
		}

		object IStateTransition.From
		{
			get => from;
		}

		object IStateTransition.To
		{
			get => to;
		}

		public TurnstileTransition(Turnstile.StateKey from, Turnstile.StateKey to, Func<bool> trigger)
		{
			trigger.ThrowIfNull(nameof(trigger));

			this.from = from;
			this.to = to;
			this.triggerEval = trigger;
		}

		public void Trigger()
		{
			onTriggered.InvokeIfNotNull(this);
		}
	}
}
