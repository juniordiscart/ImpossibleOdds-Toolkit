using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
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