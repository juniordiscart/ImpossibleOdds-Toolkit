namespace ImpossibleOdds.ReflectionCaching
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public interface IReflectionTypeMap
	{
		/// <summary>
		/// The type this reflection map handles.
		/// </summary>
		Type Type
		{
			get;
		}

		/// <summary>
		/// The base reflection map, for the base-type of the current type.
		/// </summary>
		IReflectionTypeMap BaseMap
		{
			get;
		}

		/// <summary>
		/// The cached members and attributes in this reflection map.
		/// </summary>
		IEnumerable<IMemberAttributePair> MemberAttributePairs
		{
			get;
		}

		/// <summary>
		/// Retrieve the pairs of member with attribute that match the given type of the attribute.
		/// </summary>
		/// <param name="typeOfAttribute">The type of the attribute.</param>
		/// <param name="includeBaseType">Whether or not members of the base type should be included in the result.</param>
		/// <returns>All members on the type that have the type of the attribute defined.</returns>
		IEnumerable<IMemberAttributePair> GetMembersWithAttribute(Type typeOfAttribute, bool includeBaseType = true);

		/// <summary>
		/// Gets the first member-attribute pair that matches the attribute type on the given member.
		/// </summary>
		/// <param name="member">The member to check for.</param>
		/// <param name="typeOfAttribute">The type of attribute to check for on the member.</param>
		/// <returns>The member-attribute pair for which the member and type of attribute matches.</returns>
		IMemberAttributePair GetAttributeOnMember(MemberInfo member, Type typeOfAttribute);

		/// <summary>
		/// Checks whether the member has an attribute defined on it.
		/// </summary>
		/// <param name="member">The member to check for.</param>
		/// <param name="typeOfAttribute">The type of attribute to check for on the member.</param>
		/// <returns>True if such an attribute exists on the member, false otherwise.</returns>
		bool IsAttributeDefinedOnMember(MemberInfo member, Type typeOfAttribute);
	}

	public interface IReflectionTypeMap<TMemberAttributePair> : IReflectionTypeMap
	where TMemberAttributePair : IMemberAttributePair
	{
		/// <summary>
		/// The base reflection map, for the base-type of the current type.
		/// </summary>
		new IReflectionTypeMap<TMemberAttributePair> BaseMap
		{
			get;
		}

		/// <summary>
		/// The cached members and attributes in this reflection map.
		/// </summary>
		new IEnumerable<TMemberAttributePair> MemberAttributePairs
		{
			get;
		}

		/// <summary>
		/// Gets the first member-attribute pair that matches the attribute type on the given member.
		/// </summary>
		/// <param name="member">The member to check for.</param>
		/// <param name="typeOfAttribute">The type of attribute to check for on the member.</param>
		/// <returns>The member-attribute pair for which the member and type of attribute matches.</returns>
		new TMemberAttributePair GetAttributeOnMember(MemberInfo member, Type typeOfAttribute);

		/// <summary>
		/// Retrieve the pairs of member with attribute that match the given type of the attribute.
		/// </summary>
		/// <param name="typeOfAttribute">The type of the attribute.</param>
		/// <param name="includeBaseType">Whether or not members of the base type should be included in the result.</param>
		/// <returns>All members on the type that have the type of the attribute defined.</returns>
		new IEnumerable<TMemberAttributePair> GetMembersWithAttribute(Type typeOfAttribute, bool includeBaseType = true);
	}
}
