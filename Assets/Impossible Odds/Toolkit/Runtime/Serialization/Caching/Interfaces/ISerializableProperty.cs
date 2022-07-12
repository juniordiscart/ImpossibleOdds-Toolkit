namespace ImpossibleOdds.Serialization.Caching
{
	using System.Reflection;

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
