using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;
using UnityEngine;

[XmlObject, JsonObject]
public class TrackBlueprintAction : TrackBlueprint
{
	[XmlElement, JsonField]
	public AbstractTrackItemAction action = null;
	[XmlElement("scale"), JsonField("scale")]
	public Vector3 triggerScale = Vector3.one;
}
