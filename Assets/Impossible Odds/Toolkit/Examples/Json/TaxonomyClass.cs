namespace ImpossibleOdds.Examples.Json
{
	using ImpossibleOdds.Json;

	[JsonEnumString]
	public enum TaxonomyClass
	{
		NONE,
		[JsonEnumAlias("Mammal")]
		MAMMAL,
		[JsonEnumAlias("Reptile")]
		REPTILE,
		[JsonEnumAlias("Birb")]
		BIRD
	}
}
