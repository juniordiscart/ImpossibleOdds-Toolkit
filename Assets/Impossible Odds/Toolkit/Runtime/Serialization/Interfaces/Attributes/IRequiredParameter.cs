namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Interface for parameters that implement support for required fields.
	/// </summary>
	public interface IRequiredParameter
	{
		/// <summary>
		/// Should the value be null-checked?
		/// </summary>
		bool NullCheck
		{
			get;
		}
	}
}
