namespace ImpossibleOdds.Serialization.Caching
{
	using ImpossibleOdds.ReflectionCaching;

	public interface IRequiredSerializableMember : IMemberAttributePair
	{
		/// <summary>
		/// The attribute applied to the member that marks it as being required.
		/// </summary>
		IRequiredParameter RequiredParameterAttribute
		{
			get;
		}
	}
}
