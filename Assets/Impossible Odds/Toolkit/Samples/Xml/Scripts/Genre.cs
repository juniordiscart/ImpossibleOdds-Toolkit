namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using ImpossibleOdds.Xml;

	[Flags, XmlEnumString]
	public enum Genre
	{
		[XmlEnumAlias("Undefined")]
		UNKNOWN = 0,
		[XmlEnumAlias("Action")]
		ACTION = 1 << 0,
		[XmlEnumAlias("Adventure")]
		ADVENTURE = 1 << 1,
		[XmlEnumAlias("Horror")]
		HORROR = 1 << 2,
		[XmlEnumAlias("Comedy")]
		COMEDY = 1 << 3,
		[XmlEnumAlias("Science Fiction")]
		SCI_FI = 1 << 4,
		[XmlEnumAlias("Drama")]
		DRAMA = 1 << 5,
		[XmlEnumAlias("Thriller")]
		THRILLER = 1 << 6,
		[XmlEnumAlias("Animation")]
		ANIMATION = 1 << 7,
	}
}
