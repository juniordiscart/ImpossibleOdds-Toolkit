namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;
	using ImpossibleOdds.DependencyInjection;

	[RequireComponent(typeof(CharacterController)), Injectable]
	public class Character : MonoBehaviour
	{
		[Inject]
		private IInputManager inputManager = null;
		[Inject]
		private CharacterSettings settings = null;

		private CharacterController controller = null;

		private float jumpSpeed = 0f;

		private float Gravity
		{
			get => 2f * settings.JumpHeight / Mathf.Pow(settings.JumpApexTime, 2f);
		}

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
		}

		private void Start()
		{
			if (inputManager == null)
			{
				Log.Error("No instance of {0} has been given. Character will not be able to move around.", typeof(IInputManager).Name);
			}

			if (settings == null)
			{
				Log.Error("No instance of {0} has been given. Character will not be able to move around.", typeof(CharacterSettings).Name);
			}
		}

		private void Update()
		{
			// Planar movement
			Vector3 motion = transform.right * (inputManager.Left + inputManager.Right) + transform.forward * (inputManager.Forward + inputManager.Backward);
			motion = Vector3.ClampMagnitude(motion * settings.WalkSpeed, settings.WalkSpeed);
			controller.Move(motion * Time.deltaTime);

			// Jumping
			if (inputManager.Jump)
			{
				jumpSpeed = Mathf.Sqrt(2f * settings.JumpHeight * Gravity);
			}
			else
			{
				jumpSpeed -= Gravity * Time.deltaTime;
			}

			motion = new Vector3(0f, jumpSpeed * Time.deltaTime, 0f);
			controller.Move(motion);

			if (controller.isGrounded)
			{
				jumpSpeed = 0f;
			}

			// Rotation
			float rotateValue = inputManager.TurnLeft + inputManager.TurnRight;
			transform.Rotate(Vector3.up, rotateValue * settings.RotateSpeed * Time.deltaTime);
		}
	}
}
