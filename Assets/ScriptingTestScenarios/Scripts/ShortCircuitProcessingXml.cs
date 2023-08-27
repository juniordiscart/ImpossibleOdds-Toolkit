using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ImpossibleOdds.Xml;
using UnityEngine;

public class ShortCircuitProcessingXml : MonoBehaviour
{
	[SerializeField]
	private TextAsset trackAsset = null;

	private void Start()
	{
		XDocument trackDocument = XmlProcessor.Deserialize(trackAsset.text);
		Track track = XmlProcessor.Deserialize<Track>(trackAsset.text);

		int i = 0;

		UnityEditor.EditorApplication.ExitPlaymode();
	}
}
