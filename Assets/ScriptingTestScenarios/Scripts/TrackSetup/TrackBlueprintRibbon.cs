using System.Collections.Generic;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;
using UnityEngine;

[XmlObject, JsonObject]
public class TrackBlueprintRibbon : TrackBlueprint
{
	[XmlElement, XmlListElement("attachPoints"), JsonField]
	public List<AttachPoint> attachPoints = new List<AttachPoint>();

	[XmlObject, JsonObject]
	public class AttachPoint
	{
		[XmlElement, JsonField]
		public Vector3 position = Vector3.zero;
		[XmlElement, JsonField]
		public Vector3 rotation = Vector3.zero;
		[XmlElement, JsonField]
		public string attachItemName = string.Empty;
	}
}
