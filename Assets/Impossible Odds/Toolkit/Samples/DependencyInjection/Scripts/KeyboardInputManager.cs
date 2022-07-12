namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;

	public class KeyboardInputManager : IInputManager
	{
		public float Forward
		{
			get => (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ? 1f : 0f;
		}

		public float Backward
		{
			get => (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? -1f : 0f;
		}

		public float Left
		{
			get => (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1f : 0f;
		}

		public float Right
		{
			get => (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1f : 0f;
		}

		public bool Jump
		{
			get => Input.GetKeyDown(KeyCode.Space);
		}

		public float TurnLeft
		{
			get => Input.GetMouseButton(1) ? Mathf.Clamp(Input.GetAxis("Mouse X"), 0f, 1f) : 0f;
		}

		public float TurnRight
		{
			get => Input.GetMouseButton(1) ? Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 0f) : 0f;
		}

	}
}
