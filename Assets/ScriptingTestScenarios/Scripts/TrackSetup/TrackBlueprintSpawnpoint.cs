using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class TrackBlueprintSpawnpoint : TrackBlueprint
{
	[XmlElement, JsonField]
	public AbstractSpawnpoint spawnpoint = null;
}
