namespace ImpossibleOdds.Examples.Json
{
	using UnityEngine;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class Dog : Animal
	{
		[JsonField]
		private Color furColor;
		[JsonField]
		private bool neutered;

		public Color FurColor { get => furColor; set => furColor = value; }

		public Dog()
		{
			NrOfLegs = 4;
			Classification = TaxonomyClass.MAMMAL;
		}
	}
}
