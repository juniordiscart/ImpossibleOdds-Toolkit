namespace ImpossibleOdds.Examples.Json
{
	using UnityEngine;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class Pigeon : Animal
	{
		[JsonField(Key = "Coordinates")]
		private Vector2 position;
		[JsonField(Key = "ChippedByNSA")]
		private bool isSpyPigeon = false;

		public Vector2 Position
		{
			get => position;
			set => position = value;
		}

		public bool IsSpyPigeon
		{
			get => isSpyPigeon;
			set => isSpyPigeon = value;
		}

		public Pigeon()
		{
			Classification = TaxonomyClass.BIRD;
			NrOfLegs = 2;
		}
	}
}
