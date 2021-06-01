namespace ImpossibleOdds.Examples.Json
{
	using System;
	using System.Text;
	using ImpossibleOdds.Json;

	[JsonObject,
	JsonType(typeof(Cat)),
	JsonType(typeof(Dog)),
	JsonType(typeof(Crocodile), Value = "Kroko"),
	JsonType(typeof(Pidgeon), Value = "Dove")]
	public abstract class Animal
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

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

		[OnJsonSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing animal of type {0} with name {1}.", this.GetType().Name, Name));
		}

		[OnJsonSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized animal of type {0} with name {1}.", this.GetType().Name, Name));
		}

		[OnJsonDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing animal of type {0}. No name is available yet.", this.GetType().Name));
		}

		[OnJsonDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized animal of type {0} with name {1}.", this.GetType().Name, Name));
		}

		[JsonEnumString]
		public enum TaxonomyClass
		{
			NONE,
			[JsonEnumAlias("Mammal")]
			MAMMAL,
			[JsonEnumAlias("Reptile")]
			REPTILE,
			[JsonEnumAlias("Birb")]
			BIRD
		}
	}
}
