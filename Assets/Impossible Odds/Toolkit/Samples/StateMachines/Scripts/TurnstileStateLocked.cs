namespace ImpossibleOdds.Examples.StateMachines
{
	public class TurnstileStateLocked : TurnstileState
	{
		private bool insertedCoin = false;

		public bool InsertedCoin
		{
			get { return insertedCoin; }
		}

		public void InsertCoin()
		{
			if (IsStateActive)
			{
				insertedCoin = true;
			}
		}

		public override void Exit()
		{
			base.Exit();
			insertedCoin = false;
		}
	}
}
