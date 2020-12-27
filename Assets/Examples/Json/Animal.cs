namespace ImpossibleOdds.Examples.Json
{
	using System;
	using System.Text;
	using ImpossibleOdds.Json;
	using ImpossibleOdds.Serialization;

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

		private StringBuilder log = null;

		public int NrOfLegs
		{
			get { return nrOfLegs; }
			set { nrOfLegs = value; }
		}

		public float Weight
		{
			get { return weight; }
			set { weight = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public DateTime DateOfBirth
		{
			get { return dateOfBirth; }
			set { dateOfBirth = value; }
		}

		public TaxonomyClass Classification
		{
			get { return classification; }
			set { classification = value; }
		}

		public StringBuilder Log
		{
			get { return log; }
			set { log = value; }
		}

		[OnJsonSerializing]
		private void OnSerializingAnimal()
		{
			log.AppendLine(string.Format("Serializing animal '{0}'.", name));
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
