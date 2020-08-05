namespace ImpossibleOdds.Examples.Json
{
	using System;
	using ImpossibleOdds.Json;
	using ImpossibleOdds.Serialization;

	using Debug = ImpossibleOdds.Debug;

	[JsonObject,
	JsonTypeResolve(typeof(Cat)),
	JsonTypeResolve(typeof(Dog)),
	JsonTypeResolve(typeof(Crocodile), Value = "Kroko"),
	JsonTypeResolve(typeof(Pidgeon), Value = "Dove")]
	public abstract class Animal
	{
		[JsonField]
		private int nrOfLegs;
		[JsonField]
		private float weight;
		[JsonField]
		private string name;
		[JsonField]
		private DateTime dateOfBirth;
		[JsonField]
		private TaxonomyClass classification;

		public int NrOfLegs { get => nrOfLegs; set => nrOfLegs = value; }
		public float Weight { get => weight; set => weight = value; }
		public string Name { get => name; set => name = value; }
		public DateTime DateOfBirth { get => dateOfBirth; set => dateOfBirth = value; }
		public TaxonomyClass Classification { get => classification; set => classification = value; }

		[OnJsonSerializing]
		private void OnSerializingAnimal()
		{
			Debug.Info("Serializing animal '{0}'.", name);
		}

		[OnJsonDeserialized]
		private void OnDeserializedAnimal()
		{
			Debug.Info("Deserialized animal '{0}', which is of type '{1}'.", name, this.GetType().Name);
		}

		[EnumStringSerialization]
		public enum TaxonomyClass
		{
			NONE,
			[EnumStringAlias("Mammal")]
			MAMMAL,
			[EnumStringAlias("Reptile")]
			REPTILE,
			[EnumStringAlias("Bird")]
			BIRD
		}
	}
}
