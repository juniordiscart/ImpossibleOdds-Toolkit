namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Interface for caching commonly accessed members of objects along with the defined attribute.
	/// </summary>
	public interface IMemberAttributeTuple
	{
		/// <summary>
		/// Type of the declared type of the member's value.
		/// </summary>
		Type MemberType
		{
			get;
		}

		/// <summary>
		/// The member coupled to the attribute.
		/// </summary>
		MemberInfo Member
		{
			get;
		}

		/// <summary>
		/// The attribute coupled to the member.
		/// </summary>
		Attribute Attribute
		{
			get;
		}

		/// <summary>
		/// Is the value marked as required during deserialization?
		/// </summary>
		bool IsRequiredParameter
		{
			get;
		}

		/// <summary>
		/// The required parameter data.
		/// </summary>
		IRequiredParameter RequiredParameter
		{
			get;
		}

		/// <summary>
		/// Get the value of the member that is defined on the source object.
		/// </summary>
		/// <param name="source">The source object to retrieve the member value from.</param>
		/// <returns>The value of the member of the source object.</returns>
		object GetValue(object source);

		/// <summary>
		/// Set the value of the member that is defined on the source object.
		/// </summary>
		/// <param name="source">The source object onto which the value should be set.</param>
		/// <param name="value">The value to be set on the object.</param>
		void SetValue(object source, object value);
	}

	/// <summary>
	/// Interface for caching commonly accessed members of objects along with the defined attribute.
	/// </summary>
	public interface IMemberAttributeTuple<T> : IMemberAttributeTuple
	where T : MemberInfo
	{
		new T Member
		{
			get;
		}
	}
}
