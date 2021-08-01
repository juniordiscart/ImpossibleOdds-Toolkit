namespace ImpossibleOdds.Examples.Json
{
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Json;

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

			btnDeserialize.interactable = false;
			btnSerialize.interactable = false;
			txtJson.text = string.Empty;
			txtLog.text = string.Empty;
		}

		private void OnSerialize()
		{
			jsonBuilder.Clear();

			JsonProcessor.Serialize(animalRegister, jsonBuilder, jsonOptions);
			btnDeserialize.interactable = (jsonBuilder.Length > 0);

			UpdateLog();
			txtJson.text = jsonBuilder.ToString();
		}

		private void OnDeserialize()
		{
			animalRegister = JsonProcessor.Deserialize<AnimalRegister>(txtJson.text);
			UpdateLog();

			btnSerialize.interactable = animalRegister != null;
		}

		private void OnLoadAsset()
		{
			animalRegister = JsonProcessor.Deserialize<AnimalRegister>(jsonAsset.text);
			UpdateLog();
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
	}
}
