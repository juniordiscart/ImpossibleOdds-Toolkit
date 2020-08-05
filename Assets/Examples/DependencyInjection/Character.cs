namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;
	using ImpossibleOdds.DependencyInjection;

	using Debug = ImpossibleOdds.Debug;

	[RequireComponent(typeof(CharacterController))]
	public class Character : MonoBehaviour
	{
		[Inject]
		private Camera mainCamera = null;
		[Inject]
		private IInputManager inputManager = null;
		[Inject]
		private CharacterSettings settings = null;

		[SerializeField]
		private Vector3 cameraOffset = Vector3.zero;

		private CharacterController controller = null;

		private float jumpSpeed = 0f;

		private float Gravity
		{
			get { return settings.JumpHeight / (2f * Mathf.Pow(settings.JumpApexTime, 2f)); }
		}

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
		}

		private void Start()
		{
			if (inputManager == null)
			{
				Debug.Error("No instance of {0} has been given. Character will not be able to move around.", typeof(IInputManager).Name);
			}

			if (mainCamera == null)
			{
				Debug.Error("No instance of {0} has been given. The camera will not follow the character around.", typeof(Camera).Name);
			}
			else
			{
				mainCamera.transform.SetParent(this.transform);
				mainCamera.transform.localPosition = cameraOffset;
				mainCamera.transform.LookAt(this.transform);
			}
		}

		private void Update()
		{
			// Planar movement
			Vector3 motion = new Vector3(inputManager.Left + inputManager.Right, 0f, inputManager.Forward + inputManager.Backward);
			motion = Vector3.ClampMagnitude(motion * settings.WalkSpeed, settings.WalkSpeed);
			controller.Move(motion * Time.deltaTime);

			// Jumping
			if (inputManager.JumpDown)
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
		}
	}
}
