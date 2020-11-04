namespace ImpossibleOdds.Examples.Json
{
	using System.Collections.Generic;
	using ImpossibleOdds.Json;

	[JsonObject]
	public class AnimalRegister
	{
		[JsonField(Key = "AnimalRegister")]
		private List<Animal> registeredAnimals = new List<Animal>();

		public void AddAnimal(Animal animal)
		{
			if (registeredAnimals.Contains(animal))
			{
				Log.Error("An animal can only be registered once.");
				return;
			}

			registeredAnimals.Add(animal);
		}

		[OnJsonSerializing]
		private void OnSerializingRegister()
		{
			Log.Info("Serializing the animal register.");
		}

		[OnJsonSerialized]
		private void OnSerializedRegister()
		{
			Log.Info("Serialized the animal register.");
		}

		[OnJsonDeserializing]
		private void OnDeserializingRegister()
		{
			Log.Info("Deserializing the animal register.");
		}

		[OnJsonDeserialized]
		private void OnDeserializedRegister()
		{
			Log.Info("Deserialized the animal register.");
		}


	}
}
