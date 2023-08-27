using System.Collections.Generic;
using ImpossibleOdds;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

[XmlType(typeof(TrackQuickInfo)), JsonType(typeof(TrackQuickInfo))]
public abstract class ShareableContent : LiftoffSerializable
{
	// Type of the content that is shareable.
	// When adding a new type here, make sure to add the file
	// extension to the lookup table below for easier content
	// management concerns.
	[XmlEnumString, JsonEnumString]
	public enum ContentType
	{
		NONE = 0,
		[DisplayName(Name = "Track", LocalizationKey = "shareablecontent.type.track")]
		[XmlEnumAlias("track"), JsonEnumAlias("track")]
		TRACK = 1,
		[DisplayName(Name = "Race", LocalizationKey = "shareablecontent.type.race")]
		[XmlEnumAlias("race"), JsonEnumAlias("race")]
		RACE = 2,
		[DisplayName(Name = "Drone Configuration", LocalizationKey = "shareablecontent.type.droneconfig")]
		[XmlEnumAlias("drone"), JsonEnumAlias("drone")]
		DRONE = 3,
		[DisplayName(Name = "Input Settings", LocalizationKey = "shareablecontent.type.inputsettings")]
		INPUT = 4
	}

	[XmlElement, JsonField]
	public ContentID localID = null;
	[XmlElement, JsonField]
	public ContentID managedID = null;
	[XmlElement, JsonField]
	public string name = string.Empty;
	[XmlElement, JsonField]
	public string description = string.Empty;
	[XmlElement, XmlListElement("dependency"), JsonField]
	public List<ContentID> dependencies = new List<ContentID>();

	public ShareableContent(ContentType contentType)
	{
		this.localID = new ContentID(contentType);
	}

	[XmlObject, JsonObject]
	public class ContentID
	{
		[XmlElement, JsonField]
		public string str = string.Empty;
		[XmlElement, JsonField]
		public uint version = 0;
		[XmlElement, JsonField]
		public ContentType type = ContentType.NONE;

		public ContentID()
		{
			this.str = System.Guid.NewGuid().ToString();
			this.version = 1;
			this.type = ContentType.NONE;
		}

		public ContentID(ContentType type)
		{
			this.str = System.Guid.NewGuid().ToString();
			this.version = 1;
			this.type = type;
		}
	}
}
