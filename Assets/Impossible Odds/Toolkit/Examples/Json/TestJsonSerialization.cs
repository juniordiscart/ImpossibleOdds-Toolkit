namespace ImpossibleOdds.Examples.Json
{
	using System;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Json;

	public class TestJsonSerialization : MonoBehaviour
	{
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		private TextMeshProUGUI txtJson = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;

		private AnimalRegister animalRegister = null;
		private JsonOptions jsonOptions = null;
		private StringBuilder jsonBuilder = null;
		private StringBuilder logBuilder = null;

		private void Awake()
		{
			jsonOptions = new JsonOptions();
			jsonOptions.CompactOutput = false;

			jsonBuilder = new StringBuilder();
			logBuilder = new StringBuilder();
		}

		private void Start()
		{
			Application.targetFrameRate = 60;

			btnSerialize.onClick.AddListener(OnSerialize);
			btnDeserialize.onClick.AddListener(OnDeserialize);
			btnDeserialize.interactable = false;
			txtJson.text = string.Empty;
			txtLog.text = string.Empty;
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

			animalRegister = new AnimalRegister(logBuilder);
			animalRegister.AddAnimal(minoes);
			animalRegister.AddAnimal(pickles);
			animalRegister.AddAnimal(waffles);
			animalRegister.AddAnimal(mark);
			animalRegister.AddAnimal(dundee);

			jsonBuilder.Clear();
			logBuilder.Clear();

			JsonProcessor.Serialize(animalRegister, jsonOptions, jsonBuilder);
			btnDeserialize.interactable = (jsonBuilder.Length > 0);

			txtLog.text = logBuilder.ToString();
			txtJson.text = jsonBuilder.ToString();
		}

		private void OnDeserialize()
		{
			AnimalRegister deserializedRegister = new AnimalRegister(logBuilder);
			JsonProcessor.Deserialize(deserializedRegister, jsonBuilder.ToString());
			txtLog.text = logBuilder.ToString();
		}
	}
}
