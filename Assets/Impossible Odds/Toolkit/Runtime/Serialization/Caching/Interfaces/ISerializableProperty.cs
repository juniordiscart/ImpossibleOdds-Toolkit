using System.Reflection;

namespace ImpossibleOdds.Serialization.Caching
{
	public interface ISerializableProperty : ISerializableMember
	{
		/// <summary>
		/// The property this member pertains to.
		/// </summary>
		PropertyInfo Property
		{
			get;
		}
	}
}