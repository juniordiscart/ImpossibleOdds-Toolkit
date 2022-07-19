namespace ImpossibleOdds.Examples.StateMachines
{
	using System;
	using ImpossibleOdds;
	using ImpossibleOdds.StateMachines;

	public class TrafficLightTransition : IStateTransition<TrafficLightStateKey>
	{
		private TrafficLightStateKey from;
		private TrafficLightStateKey to;
		private Func<bool> triggerCheck;

		public TrafficLightStateKey From
		{
			get => from;
		}

		public TrafficLightStateKey To
		{
			get => to;
		}

		public bool CanTrigger
		{
			get => triggerCheck();
		}

		object IStateTransition.From
		{
			get => From;
		}

		object IStateTransition.To
		{
			get => To;
		}

		public event Action<IStateTransition> onTriggered;

		public TrafficLightTransition(TrafficLightStateKey from, TrafficLightStateKey to, Func<bool> triggerCheck)
		{
			triggerCheck.ThrowIfNull(nameof(triggerCheck));
			this.from = from;
			this.to = to;
			this.triggerCheck = triggerCheck;
		}

		public void Trigger()
		{
			onTriggered.InvokeIfNotNull(this);
		}
	}
}
