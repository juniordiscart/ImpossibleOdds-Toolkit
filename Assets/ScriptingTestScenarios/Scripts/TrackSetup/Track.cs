using System.Collections.Generic;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject(RootName = "Track"), JsonObject]
public class Track : TrackQuickInfo
{
	[XmlElement, XmlListElement(nameof(TrackBlueprint)), JsonField]
	public List<TrackBlueprint> blueprints = new List<TrackBlueprint>();
	[XmlElement, JsonField]
	public bool hideDefaultSpawnpoint = false;
}
