using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class RepairPropellersTrackItemAction : AbstractTrackItemAction
{
	[XmlElement, JsonField]
	public float fullRepairTime = 1f;
}
