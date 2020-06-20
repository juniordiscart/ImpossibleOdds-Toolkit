namespace ImpossibleOdds.Testing.Json
{
	using System;
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

		public Dog(string name, DateTime dateOfBirth)
		{
			Name = name;
			DateOfBirth = dateOfBirth;
			NrOfLegs = 4;
			Classification = TaxonomyClass.MAMMAL;
		}
	}
}
