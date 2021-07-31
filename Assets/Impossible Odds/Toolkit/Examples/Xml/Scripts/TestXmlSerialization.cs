namespace ImpossibleOdds.Examples.Xml
{
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Xml;

	public class TestXmlSerialization : MonoBehaviour
	{
		[SerializeField]
		private TextAsset xmlAsset = null;
		[SerializeField]
		private Button btnLoadAsset = null;
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		private Button btnClearLog = null;
		[SerializeField]
		private TMP_InputField txtXml = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;
		[SerializeField]
		private ScrollRect scrollViewLog = null;

		private MovieDatabase movieDatabase = null;
		private XmlOptions xmlOptions = null;
		private StringBuilder xmlBuilder = null;
		private StringBuilder logBuilder = null;

		private void Awake()
		{
			xmlOptions = new XmlOptions();
			xmlOptions.CompactOutput = false;

			xmlBuilder = new StringBuilder();
			logBuilder = new StringBuilder();

			MovieDatabase.SerializationLog = logBuilder;
			Actor.SerializationLog = logBuilder;
			Producer.SerializationLog = logBuilder;
			Production.SerializationLog = logBuilder;
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
			txtXml.text = string.Empty;
			txtLog.text = string.Empty;
		}

		private void OnSerialize()
		{
			xmlBuilder.Clear();

			XmlProcessor.Serialize(movieDatabase, xmlOptions, xmlBuilder);
			btnDeserialize.interactable = (xmlBuilder.Length > 0);

			UpdateLog();
			txtXml.text = xmlBuilder.ToString();
		}

		private void OnDeserialize()
		{
			movieDatabase = XmlProcessor.Deserialize<MovieDatabase>(txtXml.text);
			UpdateLog();

			btnSerialize.interactable = movieDatabase != null;
		}

		private void OnLoadAsset()
		{
			movieDatabase = XmlProcessor.Deserialize<MovieDatabase>(xmlAsset.text);
			UpdateLog();
			txtXml.text = xmlAsset.text;

			btnSerialize.interactable = movieDatabase != null;
			btnDeserialize.interactable = !string.IsNullOrWhiteSpace(txtXml.text);
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
