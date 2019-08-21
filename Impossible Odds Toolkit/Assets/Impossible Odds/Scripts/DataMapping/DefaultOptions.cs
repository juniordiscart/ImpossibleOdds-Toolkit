namespace ImpossibleOdds.DataMapping
{
	public sealed class DefaultOptions
	{
#if IMPOSSIBLE_ODDS_DATA_MAPPING_STRICT_TYPES
		public const bool StrictTypeChecking = true;
#else
		public const bool StrictTypeChecking = false;
#endif

		public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
	}
}