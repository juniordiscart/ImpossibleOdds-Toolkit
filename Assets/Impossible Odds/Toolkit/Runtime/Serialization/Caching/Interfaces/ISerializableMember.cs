namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using ImpossibleOdds.ReflectionCaching;

	/// <summary>
	/// Interface for caching common info of a serializable member of an object.
	/// </summary>
	public interface ISerializableMember : IMemberAttributePair
	{
		/// <summary>
		/// Type of value the member stores.
		/// </summary>
		Type MemberType
		{
			get;
		}

		/// <summary>
		/// Get the value of the member that is defined on the source object.
		/// </summary>
		/// <param name="source">The source object to retrieve the value of the member from.</param>
		/// <returns>The value of the member of the source object.</returns>
		object GetValue(object source);

		/// <summary>
		/// Set the value of the member that is defined on the source object.
		/// </summary>
		/// <param name="source">The source object onto which the value should be set.</param>
		/// <param name="value">The value to be set on the object.</param>
		void SetValue(object source, object value);
	}
}
