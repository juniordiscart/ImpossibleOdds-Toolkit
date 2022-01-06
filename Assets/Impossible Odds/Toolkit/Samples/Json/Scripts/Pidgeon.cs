namespace ImpossibleOdds.Examples.Json
{
	using UnityEngine;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class Pidgeon : Animal
	{
		[JsonField(Key = "Coordinates")]
		private Vector2 position;
		[JsonField(Key = "ChippedByNSA")]
		private bool isSpyPidgeon = false;

		public Vector2 Position { get => position; set => position = value; }
		public bool IsSpyPidgeon { get => isSpyPidgeon; set => isSpyPidgeon = value; }

		public Pidgeon()
		{
			Classification = TaxonomyClass.BIRD;
			NrOfLegs = 2;
		}
	}
}
