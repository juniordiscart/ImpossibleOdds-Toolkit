using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
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