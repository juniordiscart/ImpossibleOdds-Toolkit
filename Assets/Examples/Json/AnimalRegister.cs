namespace ImpossibleOdds.Examples.Json
{
	using System.Collections.Generic;
	using ImpossibleOdds.Json;

	using Debug = ImpossibleOdds.Debug;

	[JsonObject]
	public class AnimalRegister
	{
		[JsonField(Key = "AnimalRegister")]
		private List<Animal> registeredAnimals = new List<Animal>();

		public void AddAnimal(Animal animal)
		{
			if (registeredAnimals.Contains(animal))
			{
				Debug.Error("An animal can only be registered once.");
				return;
			}

			registeredAnimals.Add(animal);
		}

		[OnJsonSerializing]
		private void OnSerializingRegister()
		{
			Debug.Info("Serializing the animal register.");
		}

		[OnJsonSerialized]
		private void OnSerializedRegister()
		{
			Debug.Info("Serialized the animal register.");
		}

		[OnJsonDeserializing]
		private void OnDeserializingRegister()
		{
			Debug.Info("Deserializing the animal register.");
		}

		[OnJsonDeserialized]
		private void OnDeserializedRegister()
		{
			Debug.Info("Deserialized the animal register.");
		}


	}
}
