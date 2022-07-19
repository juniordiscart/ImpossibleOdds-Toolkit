namespace ImpossibleOdds.Examples.StateMachines
{
	using ImpossibleOdds.StateMachines;
	using UnityEngine;
	using UnityEngine.UI;

	public class TrafficLightState : MonoBehaviour, IState
	{
		[SerializeField]
		private TrafficLightStateKey stateKey;
		[SerializeField]
		private new Image light;
		[SerializeField]
		private Color activeColor;
		[SerializeField]
		private Color inactiveColor;

		private float timer = 0f;

		public TrafficLightStateKey StateKey
		{
			get => stateKey;
		}

		public float TimeActive
		{
			get => timer;
		}

		public void Enter()
		{
			timer = 0f;
			light.color = activeColor;
			enabled = true;
		}

		public void Exit()
		{
			timer = 0f;
			light.color = inactiveColor;
			enabled = false;
		}

		public void Update()
		{
			timer += Time.deltaTime;
		}

		private void Awake()
		{
			light.color = inactiveColor;
			enabled = false;
		}
	}
}
