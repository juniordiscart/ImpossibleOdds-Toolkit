namespace ImpossibleOdds.Examples.Json
{
	using System;
	using System.Text;
	using ImpossibleOdds.Json;

	[JsonObject,
	JsonType(typeof(Cat), KeyOverride = "AnimalType", Value = AnimalType.CAT),
	JsonType(typeof(Dog), KeyOverride = "AnimalType", Value = AnimalType.DOG),
	JsonType(typeof(Crocodile), Value = "Kroko"),
	JsonType(typeof(Pigeon), Value = "Dove")]
	public abstract class Animal
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[JsonField("NrOfLegs")]
		private int nrOfLegs;
		private float weight;
		private string name;
		private DateTime dateOfBirth;
		private TaxonomyClass classification;

		public int NrOfLegs
		{
			get => nrOfLegs;
			set => nrOfLegs = value;
		}

		[JsonField]
		public float Weight
		{
			get => weight;
			set => weight = value;
		}

		[JsonField, JsonRequired(NullCheck = true)]
		public string Name
		{
			get => name;
			set => name = value;
		}

		[JsonField]
		public DateTime DateOfBirth
		{
			get => dateOfBirth;
			set => dateOfBirth = value;
		}

		[JsonField]
		public TaxonomyClass Classification
		{
			get => classification;
			set => classification = value;
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
	}
}
