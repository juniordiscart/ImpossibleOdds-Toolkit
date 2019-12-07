namespace ImpossibleOdds.Runnables
{
	public interface IRunnable
	{
		IRunner CurrentRunner
		{
			get;
			set;
		}

		void Update();
	}
}
