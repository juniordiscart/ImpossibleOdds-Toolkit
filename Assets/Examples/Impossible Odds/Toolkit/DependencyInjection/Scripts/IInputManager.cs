namespace ImpossibleOdds.Examples.DependencyInjection
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

		float TurnLeft
		{
			get;
		}

		float TurnRight
		{
			get;
		}

		bool Jump
		{
			get;
		}
	}
}
