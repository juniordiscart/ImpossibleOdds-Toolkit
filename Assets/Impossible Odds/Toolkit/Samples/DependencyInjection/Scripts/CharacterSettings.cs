namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;

	[CreateAssetMenu(menuName = "Impossible Odds/Testing/Dependency Injection/Character Settings", fileName = "newCharactersettings")]
	public class CharacterSettings : ScriptableObject
	{
		[SerializeField]
		private float walkSpeed = 1f;
		[SerializeField]
		private float jumpHeight = 1f;
		[SerializeField]
		private float jumpApexTime = 1f;
		[SerializeField]
		private float rotateSpeed = 1f;

		public float WalkSpeed
		{
			get => walkSpeed;
		}

		public float JumpHeight
		{
			get => jumpHeight;
		}

		public float JumpApexTime
		{
			get => jumpApexTime;
		}

		public float RotateSpeed
		{
			get => rotateSpeed;
		}
	}
}
