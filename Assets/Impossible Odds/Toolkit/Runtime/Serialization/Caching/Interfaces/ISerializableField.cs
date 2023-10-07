using System.Reflection;

namespace ImpossibleOdds.Serialization.Caching
{
	public interface ISerializableField : ISerializableMember
	{
		/// <summary>
		/// The field this member pertains to.
		/// </summary>
		FieldInfo Field
		{
			get;
		}
	}
}