namespace ImpossibleOdds.Examples.Json
{
	using System;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Json;
	using System.Collections.Generic;

	public class TestJsonSerialization : MonoBehaviour
	{
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		private TMP_InputField txtJson = null;
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

			AnimalRegister.SerializationLog = logBuilder;
			Animal.SerializationLog = logBuilder;
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
			List<Animal> animals = new List<Animal>()
			{
				new Cat()
				{
					Name = "Minoes",
					Chipped = false,
					Weight = 4.1f,
				},
				new Cat()
				{
					Name = "Pickles",
					Chipped = true,
					FurColor = Color.white,
					Weight = 3.6f,
				},
				new Dog()
				{
					Name = "Waffles",
					DateOfBirth = new DateTime(1990, 6, 25),
					FurColor = Color.yellow,
					Weight = 28.5f,
				},
				new Pidgeon()
				{
					Name = "Mark",
					IsSpyPidgeon = true,
					Position = new Vector2(39.0458f, 76.6413f),
				},
				new Crocodile()
				{
					Name = "Dundee",
				}
			};

			animalRegister = new AnimalRegister();
			animalRegister.AddAnimals(animals);

			jsonBuilder.Clear();
			logBuilder.Clear();

			JsonProcessor.Serialize(animalRegister, jsonOptions, jsonBuilder);
			btnDeserialize.interactable = (jsonBuilder.Length > 0);

			txtLog.text = logBuilder.ToString();
			txtJson.text = jsonBuilder.ToString();
		}

		private void OnDeserialize()
		{
			AnimalRegister animalRegister = JsonProcessor.Deserialize<AnimalRegister>(txtJson.text);
			txtLog.text = logBuilder.ToString();
		}
	}
}
