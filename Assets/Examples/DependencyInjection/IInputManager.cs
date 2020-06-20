namespace ImpossibleOdds.Testing.DependencyInjection
{
	public interface IInputManager
	{
		float Forward
		{
			get;
		}

		float Backward
		{
			get;
		}

		float Left
		{
			get;
		}

		float Right
		{
			get;
		}

		bool JumpDown
		{
			get;
		}
	}
}
