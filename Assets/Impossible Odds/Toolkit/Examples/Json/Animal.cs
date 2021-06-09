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

		[JsonField("NrOfLegs")]
		private int nrOfLegs;
		private float weight;
		private string name;
		private DateTime dateOfBirth;
		private TaxonomyClass classification;

		public int NrOfLegs
		{
			get { return nrOfLegs; }
			set { nrOfLegs = value; }
		}

		[JsonField]
		public float Weight
		{
			get { return weight; }
			set { weight = value; }
		}

		[JsonField, JsonRequired(NullCheck = true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[JsonField]
		public DateTime DateOfBirth
		{
			get { return dateOfBirth; }
			set { dateOfBirth = value; }
		}

		[JsonField]
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
	}
}
