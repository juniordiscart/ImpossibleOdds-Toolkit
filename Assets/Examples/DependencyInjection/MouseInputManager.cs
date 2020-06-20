namespace ImpossibleOdds.Testing.DependencyInjection
{
	using UnityEngine;

	public class MouseInputManager : IInputManager
	{
		public float Forward
		{
			get { return Mathf.Clamp(Input.GetAxis("Mouse Y"), 0f, 1f); }
		}

		public float Backward
		{
			get { return Mathf.Clamp(Input.GetAxis("Mouse Y"), 0f, -1f); }
		}

		public float Left
		{
			get { return Mathf.Clamp(Input.GetAxis("Mouse X"), 0f, -1f); }
		}

		public float Right
		{
			get { return Mathf.Clamp(Input.GetAxis("Mouse X"), 0f, 1f); }
		}

		public bool JumpDown
		{
			get { return Input.GetMouseButtonDown(0); }
		}
	}
}
