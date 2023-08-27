using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class NamedDroneSpawnpoint : AbstractSpawnpoint
{
	[XmlElement, JsonField]
	public string name = string.Empty;
}
