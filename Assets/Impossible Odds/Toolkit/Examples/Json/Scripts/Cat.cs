namespace ImpossibleOdds.Examples.Json
{
	using UnityEngine;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class Cat : Animal
	{
		[JsonField]
		private Color32 furColor;
		[JsonField]
		private bool chipped;

		public Cat()
		{
			NrOfLegs = 4;
			Classification = TaxonomyClass.MAMMAL;
		}

		public bool Chipped { get => chipped; set => chipped = value; }
		public Color32 FurColor { get => furColor; set => furColor = value; }
	}
}
