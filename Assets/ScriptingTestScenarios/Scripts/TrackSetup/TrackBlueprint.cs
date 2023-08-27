using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;
using UnityEngine;

[XmlType(typeof(TrackBlueprintFlag)), JsonType(typeof(TrackBlueprintFlag))]
[XmlType(typeof(TrackBlueprintRibbon)), JsonType(typeof(TrackBlueprintRibbon))]
[XmlType(typeof(TrackBlueprintAction)), JsonType(typeof(TrackBlueprintAction))]
[XmlType(typeof(TrackBlueprintFlexibleFlag)), JsonType(typeof(TrackBlueprintFlexibleFlag))]
[XmlType(typeof(TrackBlueprintSpawnpoint)), JsonType(typeof(TrackBlueprintSpawnpoint))]
[XmlObject, JsonObject]
public class TrackBlueprint
{
	// assigning is always done (by TrackEditor)
	[XmlElement, JsonField]
	public string itemID = string.Empty;
	[XmlElement, JsonField]
	public int instanceID = -1;

	// assigning is to be implemented manually per blueprint type in SaveData()
	[XmlElement, JsonField]
	public Vector3 position = Vector3.zero;
	[XmlElement, JsonField]
	public Vector3 rotation = Vector3.zero;
}
