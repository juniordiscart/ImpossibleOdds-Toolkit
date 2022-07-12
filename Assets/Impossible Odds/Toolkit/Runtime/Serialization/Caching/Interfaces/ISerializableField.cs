namespace ImpossibleOdds.Serialization.Caching
{
	using System.Reflection;

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
