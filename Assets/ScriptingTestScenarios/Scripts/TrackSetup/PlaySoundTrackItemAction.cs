using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class PlaySoundTrackItemAction : AbstractTrackItemAction
{
	[XmlElement, JsonField]
	public string soundfile = string.Empty;
}
