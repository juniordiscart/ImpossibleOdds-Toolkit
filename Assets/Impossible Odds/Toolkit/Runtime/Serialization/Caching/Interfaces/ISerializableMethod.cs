namespace ImpossibleOdds.Serialization.Caching
{
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public interface ISerializableMethod : IMemberAttributePair
	{
		/// <summary>
		/// The method this member pertains to.
		/// </summary>
		MethodInfo Method
		{
			get;
		}
	}
}
