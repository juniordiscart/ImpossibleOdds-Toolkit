namespace ImpossibleOdds.Testing.DependencyInjection
{
	using UnityEngine;

	public class KeyboardInputManager : IInputManager
	{
		public float Forward
		{
			get { return Input.GetKey(KeyCode.UpArrow) ? 1f : 0f; }
		}

		public float Backward
		{
			get { return Input.GetKey(KeyCode.DownArrow) ? -1f : 0f; }
		}

		public float Left
		{
			get { return Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f; }
		}

		public float Right
		{
			get { return Input.GetKey(KeyCode.RightArrow) ? 1f : 0f; }
		}

		public bool JumpDown
		{
			get { return Input.GetKeyDown(KeyCode.Space); }
		}
	}
}
