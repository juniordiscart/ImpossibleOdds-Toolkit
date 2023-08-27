using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;
using UnityEngine;

[XmlObject, JsonObject]
public class TrackBlueprintFlexibleFlag : TrackBlueprintFlag
{
	[XmlElement, JsonField]
	public Vector3 scale = Vector3.one;
}
