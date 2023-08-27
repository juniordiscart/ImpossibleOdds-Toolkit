using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlObject, JsonObject]
public class ChargeBatteryItemAction : AbstractTrackItemAction
{
	[XmlElement, JsonField]
	public float fullRechargeTime = 1f;
}
