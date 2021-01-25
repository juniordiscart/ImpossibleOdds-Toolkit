namespace ImpossibleOdds.Examples.Json
{
	using System.Collections.Generic;
	using System.Text;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class AnimalRegister
	{
		[JsonField(Key = "AnimalRegister")]
		private List<Animal> registeredAnimals = new List<Animal>();
		private StringBuilder log = null;

		public AnimalRegister(StringBuilder log)
		{
			this.log = log;
		}

		public void AddAnimal(Animal animal)
		{
			if (registeredAnimals.Contains(animal))
			{
				Log.Error("An animal can only be registered once.");
				return;
			}

			registeredAnimals.Add(animal);
			animal.Log = log;
		}

		[OnJsonSerializing]
		private void OnSerializingRegister()
		{
			log.AppendLine("Serializing the animal register.");
		}

		[OnJsonSerialized]
		private void OnSerializedRegister()
		{
			log.AppendLine("Serialized the animal register.");
		}

		[OnJsonDeserializing]
		private void OnDeserializingRegister()
		{
			log.AppendLine("Deserializing the animal register.");
		}

		[OnJsonDeserialized]
		private void OnDeserializedRegister()
		{
			log.AppendLine("Deserialized the animal register.");
		}
	}
}
