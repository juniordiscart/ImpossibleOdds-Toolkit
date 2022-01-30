namespace ImpossibleOdds.StateMachines
{
	public interface IState
	{
		void Enter();
		void Update();
		void Exit();
	}
}
