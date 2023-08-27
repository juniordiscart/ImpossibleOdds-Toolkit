using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class ShowTextTrackItemAction : AbstractTrackItemAction
{
	[XmlElement, JsonField]
	public string displayText = string.Empty;
}
