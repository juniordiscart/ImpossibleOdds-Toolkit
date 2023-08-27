using System;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

public abstract class LiftoffSerializable
{
	[XmlElement, JsonField]
	public Version gameVersion = new Version();
}
