namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;

	public class KeyboardInputManager : IInputManager
	{
		public float Forward
		{
			get { return (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ? 1f : 0f; }
		}

		public float Backward
		{
			get { return (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? -1f : 0f; }
		}

		public float Left
		{
			get { return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1f : 0f; }
		}

		public float Right
		{
			get { return (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1f : 0f; }
		}

		public bool Jump
		{
			get { return Input.GetKeyDown(KeyCode.Space); }
		}

		public float TurnLeft
		{
			get { return Input.GetMouseButton(1) ? Mathf.Clamp(Input.GetAxis("Mouse X"), 0f, 1f) : 0f; }
		}

		public float TurnRight
		{
			get { return Input.GetMouseButton(1) ? Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 0f) : 0f; }
		}

	}
}
