namespace ImpossibleOdds.Examples.Json
{
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Json;
	using System.Diagnostics;

	public class TestJsonSerialization : MonoBehaviour
	{
		[SerializeField]
		private TextAsset jsonAsset = null;
		[SerializeField]
		private Button btnLoadAsset = null;
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		private Button btnClearLog = null;
		[SerializeField]
		private Toggle toggleEnableParallelProcessing = null;
		[SerializeField]
		private Toggle toggleCompactOutput = null;
		[SerializeField]
		private TMP_InputField txtJson = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;
		[SerializeField]
		private ScrollRect scrollViewLog = null;

		private AnimalRegister animalRegister = null;
		private JsonOptions jsonOptions = null;
		private StringBuilder jsonBuilder = null;
		private StringBuilder logBuilder = null;

		private void Awake()
		{
			jsonOptions = new JsonOptions();
			jsonOptions.CompactOutput = false;
			jsonOptions.SerializationDefinition = new JsonSerializationDefinition(false);

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
			btnLoadAsset.onClick.AddListener(OnLoadAsset);
			btnClearLog.onClick.AddListener(OnClearLog);

			toggleCompactOutput.isOn = jsonOptions.CompactOutput;
			toggleEnableParallelProcessing.isOn = (jsonOptions.SerializationDefinition as JsonSerializationDefinition).ParallelProcessingEnabled;
			toggleCompactOutput.onValueChanged.AddListener(OnCompactOutput);
			toggleEnableParallelProcessing.onValueChanged.AddListener(OnParallelProcessing);

			btnDeserialize.interactable = false;
			btnSerialize.interactable = false;
			txtJson.text = string.Empty;
			txtLog.text = string.Empty;
		}

		private void OnSerialize()
		{
			jsonBuilder.Clear();

			Stopwatch serializationTimer = Stopwatch.StartNew();
			JsonProcessor.Serialize(animalRegister, jsonBuilder, jsonOptions);
			serializationTimer.Stop();
			logBuilder.AppendLine(string.Format("Serialized the animal register in {0} ms.", serializationTimer.ElapsedMilliseconds));

			UpdateLog();
			txtJson.text = jsonBuilder.ToString();
			btnDeserialize.interactable = (jsonBuilder.Length > 0);
		}

		private void OnDeserialize()
		{
			Stopwatch deserializationTimer = Stopwatch.StartNew();
			animalRegister = JsonProcessor.Deserialize<AnimalRegister>(txtJson.text, jsonOptions);
			deserializationTimer.Stop();
			logBuilder.AppendLine(string.Format("Deserialized the animal register in {0}ms.", deserializationTimer.ElapsedMilliseconds));

			UpdateLog();
			btnSerialize.interactable = animalRegister != null;
		}

		private void OnLoadAsset()
		{
			txtJson.text = jsonAsset.text;
			btnSerialize.interactable = animalRegister != null;
			btnDeserialize.interactable = !string.IsNullOrWhiteSpace(txtJson.text);
		}

		private void UpdateLog()
		{
			txtLog.text = logBuilder.ToString();
			Canvas.ForceUpdateCanvases();
			scrollViewLog.verticalNormalizedPosition = 0f;
		}

		private void OnClearLog()
		{
			logBuilder.Clear();
			txtLog.text = string.Empty;
		}

		private void OnCompactOutput(bool isOn)
		{
			jsonOptions.CompactOutput = isOn;
		}

		private void OnParallelProcessing(bool isOn)
		{
			(jsonOptions.SerializationDefinition as JsonSerializationDefinition).ParallelProcessingEnabled = isOn;
		}
	}
}
