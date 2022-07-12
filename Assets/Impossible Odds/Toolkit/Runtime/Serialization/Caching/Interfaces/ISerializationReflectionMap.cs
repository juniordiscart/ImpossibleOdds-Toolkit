namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public interface ISerializationReflectionMap : IReflectionMap
	{
		/// <summary>
		/// Get the type resolve parameters defined for this type.
		/// </summary>
		/// <param name="attributeType">Type of the type resolve parameter to look for.</param>
		/// <returns>All type resolve parameters that match the type requirement.</returns>
		ITypeResolveParameter[] GetTypeResolveParameters(Type attributeType);

		/// <summary>
		/// Get all serializable members on this type with the defined attribute.
		/// </summary>
		/// <param name="attributeType">The serialization attribute to filter for.</param>
		/// <returns>All members with the serialization attribute defined.</returns>
		ISerializableMember[] GetSerializableMembers(Type attributeType);

		/// <summary>
		/// Get all members that are considered to be required during (de)serialization.
		/// </summary>
		/// <param name="attributeType">The attribute that marks a member as a required parameter during (de)serialization.</param>
		/// <returns>All required members with the attribute defined.</returns>
		IRequiredSerializableMember[] GetRequiredMembers(Type attributeType);

		/// <summary>
		/// Checks whether the member is considered to be required during (de)serialization.
		/// </summary>
		/// <param name="member">The member to check for.</param>
		/// <param name="attributeType">The type of the attribute that denotes the member is required.</param>
		/// <returns>True if the member is considered to be a required, false otherwise.</returns>
		bool IsMemberRequired(MemberInfo member, Type attributeType);

		/// <summary>
		/// Tries to retrieve the required information about the given member.
		/// </summary>
		/// <param name="member">The member for which to try and get the required information.</param>
		/// <param name="attributeType">The type of attribute that denotes the member is required.</param>
		/// <param name="requiredMemberInfo">The required member information.</param>
		/// <returns>True if the required information was found, false otherwise.</returns>
		bool TryGetRequiredMemberInfo(MemberInfo member, Type attributeType, out IRequiredSerializableMember requiredMemberInfo);

		/// <summary>
		/// Get all serialization callback methods decorated with the given attribute type.
		/// </summary>
		/// <param name="attributeType">The attribute type denoting a callback method.</param>
		/// <returns>All callback methods decorated with the callback attribute.</returns>
		ISerializationCallback[] GetSerializationCallbackMethods(Type attributeType);
	}
}
