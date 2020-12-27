namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;

	public class WorldBounds : MonoBehaviour
	{
		[SerializeField]
		private Bounds worldBounds = new Bounds(Vector3.zero, Vector3.one);
		[SerializeField]
		private Transform target = null;

		private void Update()
		{
			if (!worldBounds.Contains(target.position))
			{
				target.position = worldBounds.ClosestPoint(target.position);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
		}
	}
}
