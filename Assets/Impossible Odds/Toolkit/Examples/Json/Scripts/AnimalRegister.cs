namespace ImpossibleOdds.Examples.Json
{
	using System.Collections.Generic;
	using System.Text;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class AnimalRegister
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[JsonField(Key = "AnimalRegister")]
		private List<Animal> registeredAnimals = new List<Animal>();

		public AnimalRegister()
		{ }

		public void AddAnimal(Animal animal)
		{
			registeredAnimals.Add(animal);
		}

		public void AddAnimals(IEnumerable<Animal> animals)
		{
			registeredAnimals.AddRange(animals);
		}

		[OnJsonSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine("Serializing the animal register.");
		}

		[OnJsonSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine("Serialized the animal register.");
		}

		[OnJsonDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine("Deserializing the animal register.");
		}

		[OnJsonDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine("Deserialized the animal register.");
		}
	}
}
