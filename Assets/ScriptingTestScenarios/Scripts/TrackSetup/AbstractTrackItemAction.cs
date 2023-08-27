using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlType(typeof(ChargeBatteryItemAction)),
 XmlType(typeof(KillDroneTrackItemAction)),
 XmlType(typeof(PlaySoundTrackItemAction)),
 XmlType(typeof(ShowTextTrackItemAction)),
 XmlType(typeof(RepairPropellersTrackItemAction))]
[JsonType(typeof(ChargeBatteryItemAction)),
 JsonType(typeof(KillDroneTrackItemAction)),
 JsonType(typeof(PlaySoundTrackItemAction)),
 JsonType(typeof(ShowTextTrackItemAction)),
 JsonType(typeof(RepairPropellersTrackItemAction))]
public abstract class AbstractTrackItemAction
{ }
