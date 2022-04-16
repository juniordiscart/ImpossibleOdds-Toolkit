namespace ImpossibleOdds.ReflectionCaching
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds;

	public abstract class TypeReflectionMap<TMemberAttributePair> : IReflectionTypeMap<TMemberAttributePair>
	where TMemberAttributePair : IMemberAttributePair
	{
		public static readonly Type AttributeType = typeof(Attribute);
		public static readonly Type ObjectType = typeof(object);

		private readonly Type type;
		private readonly TypeReflectionMap<TMemberAttributePair> baseMap = null;

		/// <inheritdoc />
		public Type Type
		{
			get { return type; }
		}

		/// <inheritdoc />
		public IReflectionTypeMap<TMemberAttributePair> BaseMap
		{
			get { return baseMap; }
		}

		/// <inheritdoc />
		IReflectionTypeMap IReflectionTypeMap.BaseMap
		{
			get { return baseMap; }
		}

		/// <inheritdoc />
		public abstract IEnumerable<TMemberAttributePair> MemberAttributePairs
		{
			get;
		}

		/// <inheritdoc />
		IEnumerable<IMemberAttributePair> IReflectionTypeMap.MemberAttributePairs
		{
			get { return (IEnumerable<IMemberAttributePair>)MemberAttributePairs; }
		}

		public bool IsInterface
		{
			get { return type.IsInterface; }
		}

		public bool IsAbstract
		{
			get { return type.IsAbstract; }
		}

		public bool IsInterfaceOrAbstract
		{
			get { return IsInterface || IsAbstract; }
		}

		protected TypeReflectionMap(Type type)
		{
			type.ThrowIfNull(nameof(type));
			this.type = type;
		}

		protected TypeReflectionMap(Type type, TypeReflectionMap<TMemberAttributePair> parentMap)
		{
			type.ThrowIfNull(nameof(type));

			if ((parentMap != null) && !parentMap.Type.IsAssignableFrom(type))
			{
				throw new ReflectionCachingException("Parent reflection map of type {0} is not a base type of {1}.", parentMap.Type.Name, type.Name);
			}

			this.type = type;
			this.baseMap = parentMap;
		}

		/// <inheritdoc />
		public virtual IEnumerable<TMemberAttributePair> GetMembersWithAttribute(Type typeOfAttribute, bool includeBaseType)
		{
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (!IsAttributeType(typeOfAttribute))
			{
				throw new ReflectionCachingException("The type '{0}' is not an attribute.", typeOfAttribute.Name);
			}

			foreach (TMemberAttributePair memberAttributePair in MemberAttributePairs)
			{
				if (typeOfAttribute.IsInstanceOfType(memberAttributePair.Attribute))
				{
					yield return memberAttributePair;
				}
			}

			if (includeBaseType && (baseMap != null))
			{
				foreach (TMemberAttributePair memberAttributePair in baseMap.GetMembersWithAttribute(typeOfAttribute, includeBaseType))
				{
					yield return memberAttributePair;
				}
			}
		}

		/// <inheritdoc />
		public virtual bool IsAttributeDefinedOnMember(MemberInfo member, Type typeOfAttribute)
		{
			member.ThrowIfNull(nameof(member));
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (!IsAttributeType(typeOfAttribute))
			{
				throw new ReflectionCachingException("The type '{0}' is not an attribute.", typeOfAttribute.Name);
			}

			if (member.DeclaringType == Type)
			{
				return MemberAttributePairs.Any(m => (m.Member == member) && (m.TypeOfAttribute == typeOfAttribute));
			}
			else if (member.DeclaringType.IsSubclassOf(Type) && (BaseMap != null))
			{
				return BaseMap.IsAttributeDefinedOnMember(member, typeOfAttribute);
			}
			else
			{
				throw new ReflectionCachingException("The member to check for does not originate from within the type-tree defined for this reflection cache.");
			}
		}

		/// <inheritdoc />
		public virtual TMemberAttributePair GetAttributeOnMember(MemberInfo member, Type typeOfAttribute)
		{
			member.ThrowIfNull(nameof(member));
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (!IsAttributeType(typeOfAttribute))
			{
				throw new ReflectionCachingException("The type '{0}' is not an attribute.", typeOfAttribute.Name);
			}

			if (member.DeclaringType == Type)
			{
				return MemberAttributePairs.FirstOrDefault(m => (m.Member == member) && (m.TypeOfAttribute == typeOfAttribute));
			}
			else if (member.DeclaringType.IsSubclassOf(Type) && (BaseMap != null))
			{
				return BaseMap.GetAttributeOnMember(member, typeOfAttribute);
			}
			else
			{
				throw new ReflectionCachingException("The member to check for does not originate from within the type-tree defined for this reflection cache.");
			}
		}

		/// <inheritdoc />
		IMemberAttributePair IReflectionTypeMap.GetAttributeOnMember(MemberInfo member, Type typeOfAttribute)
		{
			return GetAttributeOnMember(member, typeOfAttribute);
		}

		/// <inheritdoc />
		IEnumerable<IMemberAttributePair> IReflectionTypeMap.GetMembersWithAttribute(Type typeOfAttribute, bool includeBaseType)
		{
			return (IEnumerable<IMemberAttributePair>)GetMembersWithAttribute(typeOfAttribute, includeBaseType);
		}

		private bool IsAttributeType(Type typeOfAttribute)
		{
			return AttributeType.IsAssignableFrom(typeOfAttribute);
		}
	}
}
