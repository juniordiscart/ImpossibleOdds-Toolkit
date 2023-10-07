namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Defines an index-based parameter during the data (de)serialization process.
	/// </summary>
	public interface ISequenceParameter
	{
		/// <summary>
		/// The index at which the data should be stored or retrieved during (de)serialization.
		/// </summary>
		int Index
		{
			get;
		}
	}
}