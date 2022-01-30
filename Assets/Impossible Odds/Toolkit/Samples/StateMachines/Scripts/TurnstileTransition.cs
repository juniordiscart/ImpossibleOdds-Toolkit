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
			get { return from; }
		}

		public Turnstile.StateKey To
		{
			get { return to; }
		}

		public bool CanTrigger
		{
			get { return triggerEval(); }
		}

		object IStateTransition.From
		{
			get { return from; }
		}

		object IStateTransition.To
		{
			get { return to; }
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
