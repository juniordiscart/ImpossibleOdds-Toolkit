namespace ImpossibleOdds.Testing.Json
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Json;

	using Debug = ImpossibleOdds.Debug;


	public class TestJsonSerialization : MonoBehaviour
	{
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		// private Text txtJsonResult = null;
		private TextMeshProUGUI txtJsonResult = null;


		private string serializedResult = string.Empty;
		private AnimalRegister animalRegister = null;

		private void Start()
		{
			Application.targetFrameRate = 60;

			btnSerialize.onClick.AddListener(OnSerialize);
			btnDeserialize.onClick.AddListener(OnDeserialize);
			btnDeserialize.interactable = false;
			txtJsonResult.text = "No data serialized.";
		}

		private void OnSerialize()
		{
			Cat minoes = new Cat("Minoes");
			minoes.Chipped = false;
			Cat pickles = new Cat("Pickles");
			pickles.Chipped = true;
			pickles.FurColor = Color.white;
			Dog waffles = new Dog("Waffles", new DateTime(1990, 6, 25));
			waffles.FurColor = Color.yellow;
			Pidgeon mark = new Pidgeon("Mark");
			mark.IsSpyPidgeon = true;
			mark.Position = new Vector2(89.123f, 73.456f);
			Crocodile dundee = new Crocodile();
			dundee.Name = "Dundee";

			animalRegister = new AnimalRegister();
			animalRegister.AddAnimal(minoes);
			animalRegister.AddAnimal(pickles);
			animalRegister.AddAnimal(waffles);
			animalRegister.AddAnimal(mark);
			animalRegister.AddAnimal(dundee);

			JsonOptions options = new JsonOptions();
			options.Minify = false;
			serializedResult = JsonProcessor.Serialize(animalRegister, options);
			txtJsonResult.text = serializedResult;

			btnDeserialize.interactable = !string.IsNullOrWhiteSpace(serializedResult);
		}

		private void OnDeserialize()
		{
			AnimalRegister deserializedRegister = new AnimalRegister();
			JsonProcessor.Deserialize(deserializedRegister, serializedResult);
		}
	}
}
