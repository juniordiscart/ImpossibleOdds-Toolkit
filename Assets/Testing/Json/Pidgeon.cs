namespace ImpossibleOdds.Testing.Json
{
	using UnityEngine;
	using ImpossibleOdds.Json;

	public class Pidgeon : Animal
	{
		[JsonField(Key = "Coordinates")]
		private Vector2 position;
		[JsonField(Key = "ChippedByNSA")]
		private bool isSpyPidgeon = false;

		public Vector2 Position { get => position; set => position = value; }
		public bool IsSpyPidgeon { get => isSpyPidgeon; set => isSpyPidgeon = value; }

		public Pidgeon(string name)
		{
			Name = name;
			Classification = TaxonomyClass.BIRD;
			Weight = 0.235f;
			NrOfLegs = 2;
		}
	}
}
