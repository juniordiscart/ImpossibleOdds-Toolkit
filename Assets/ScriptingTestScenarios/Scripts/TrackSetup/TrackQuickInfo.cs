using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject(RootName = "Track"), JsonObject]
public class TrackQuickInfo : ShareableContent
{
	[XmlElement, JsonField]
	public bool containsCollectableItems = false;

	[XmlElement, JsonField]
	public string environment = string.Empty;

	public TrackQuickInfo()
	: base(ContentType.TRACK)
	{ }
}
