namespace ImpossibleOdds.Examples.StateMachines
{
	public class TurnstileStateUnlocked : TurnstileState
	{
		private bool handleTurned = false;

		public bool HandleTurned
		{
			get => handleTurned;
		}

		public void TurnHandle()
		{
			if (IsStateActive)
			{
				handleTurned = true;
			}
		}

		public override void Exit()
		{
			base.Exit();
			handleTurned = false;
		}
	}
}
