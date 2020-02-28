namespace ImpossibleOdds.Runnables
{
	public interface IFixedRunnable
	{
		IFixedRunner CurrentRunner
		{
			get;
			set;
		}

		void FixedUpdate();
	}
}
