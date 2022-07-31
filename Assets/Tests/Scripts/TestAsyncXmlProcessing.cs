using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ImpossibleOdds;
using ImpossibleOdds.Xml;
using UnityEngine;

public class TestAsyncXmlProcessing : MonoBehaviour
{
	[SerializeField]
	private List<TextAsset> xmlAssets;

	private async void Start()
	{
		XmlOptions xmlOptions = new XmlOptions();
		xmlOptions.ReaderSettings = new XmlReaderSettings();
		xmlOptions.ReaderSettings.DtdProcessing = DtdProcessing.Ignore;

		Task<XDocument>[] deserializationTasks = new Task<XDocument>[xmlAssets.Count];
		for (int i = 0; i < xmlAssets.Count; ++i)
		{
			deserializationTasks[i] = XmlProcessor.DeserializeAsync(xmlAssets[i].text, xmlOptions);
		}

		await Task.WhenAll(deserializationTasks);

		Task<string>[] serializationTasks = new Task<string>[xmlAssets.Count];
		for (int i = 0; i < xmlAssets.Count; ++i)
		{
			serializationTasks[i] = XmlProcessor.SerializeAsync(deserializationTasks[i].Result, xmlOptions);
		}

		await Task.WhenAll(serializationTasks);

		for (int i = 0; i < xmlAssets.Count; ++i)
		{
			Log.Info("Reserialized asset asynchronously:\n{0}", serializationTasks[i].Result);
		}
	}
}
