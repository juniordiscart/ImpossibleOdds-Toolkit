namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds;
	using ImpossibleOdds.ReflectionCaching;

	/// <summary>
	/// An abstract reflection map for serialization purposes.
	/// </summary>
	public class SerializationReflectionMap : ISerializationReflectionMap
	{
		private readonly Type type = null;
		private readonly ConcurrentDictionary<Type, Attribute[]> typeDefinedAttributes = new ConcurrentDictionary<Type, Attribute[]>();
		private readonly ConcurrentDictionary<Type, ITypeResolveParameter[]> typeResolveParameters = new ConcurrentDictionary<Type, ITypeResolveParameter[]>();
		private readonly ConcurrentDictionary<Type, ISerializableMember[]> serializableMembers = new ConcurrentDictionary<Type, ISerializableMember[]>();
		private readonly ConcurrentDictionary<Type, ISerializationCallback[]> serializationCallbacks = new ConcurrentDictionary<Type, ISerializationCallback[]>();
		private readonly ConcurrentDictionary<Type, IRequiredSerializableMember[]> requiredMembers = new ConcurrentDictionary<Type, IRequiredSerializableMember[]>();

		public SerializationReflectionMap(Type type)
		{
			type.ThrowIfNull(nameof(type));
			this.type = type;
		}

		/// <inheritdoc />
		public Type Type
		{
			get => type;
		}

		public void Clear()
		{
			typeDefinedAttributes.Clear();
			typeResolveParameters.Clear();
			serializableMembers.Clear();
			serializationCallbacks.Clear();
			requiredMembers.Clear();
		}

		/// <inheritdoc />
		public ITypeResolveParameter[] GetTypeResolveParameters(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindTypeResolveParameters(attributeType);
		}

		/// <inheritdoc />
		public ISerializableMember[] GetSerializableMembers(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindSerializableMembers(attributeType);
		}

		/// <inheritdoc />
		public ISerializationCallback[] GetSerializationCallbackMethods(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindSerializationCallbackMethods(attributeType);
		}

		/// <inheritdoc />
		public IRequiredSerializableMember[] GetRequiredMembers(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindRequiredMembers(attributeType);
		}

		/// <inheritdoc />
		public bool IsMemberRequired(MemberInfo member, Type attributeType)
		{
			member.ThrowIfNull(nameof(member));
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindRequiredMembers(attributeType).Any(rm => rm.Member == member);
		}

		/// <inheritdoc />
		public bool TryGetRequiredMemberInfo(MemberInfo member, Type attributeType, out IRequiredSerializableMember requiredMemberInfo)
		{
			member.ThrowIfNull(nameof(member));
			attributeType.ThrowIfNull(nameof(attributeType));

			foreach (IRequiredSerializableMember rp in FindRequiredMembers(attributeType))
			{
				if (rp.Member == member)
				{
					requiredMemberInfo = rp;
					return true;
				}
			}

			requiredMemberInfo = null;
			return false;
		}

		/// <summary>
		/// Retrieve the type defining attributes of a specific attribute type.
		/// </summary>
		/// <param name="attributeType">The type of the type-defining attribute.</param>
		/// <returns>An array of type defining attributes.</returns>
		public Attribute[] GetTypeDefinedAttributes(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindTypeDefinedAttributes(attributeType);
		}

		private Attribute[] FindTypeDefinedAttributes(Type attributeType)
		{
			if (typeDefinedAttributes.TryGetValue(attributeType, out Attribute[] r))
			{
				return r;
			}

			Attribute[] attributes = TypeReflectionUtilities.FindAllTypeDefinedAttributes(Type, attributeType, true).ToArray();
			return typeDefinedAttributes.GetOrAdd(attributeType, !attributes.IsNullOrEmpty() ?
				attributes :
				Array.Empty<Attribute>());
		}

		private ITypeResolveParameter[] FindTypeResolveParameters(Type attributeType)
		{
			if (typeResolveParameters.TryGetValue(attributeType, out ITypeResolveParameter[] result))
			{
				return result;
			}

			Attribute[] typeDefinedAttributes = FindTypeDefinedAttributes(attributeType);
			Attribute[] typeResolveAttributes = typeDefinedAttributes.Where(attr => !Type.IsSubclassOf(((ITypeResolveParameter)attr).Target)).ToArray();
			List<ITypeResolveParameter> validTypeResolveParameters = new List<ITypeResolveParameter>(typeResolveAttributes.Cast<ITypeResolveParameter>());

			// Find backtracking type resolve attributes defined on this type itself.
			foreach (ITypeResolveParameter typeResolveParameter in typeDefinedAttributes.Where(attr => Type.IsSubclassOf(((ITypeResolveParameter)attr).Target)).Cast<ITypeResolveParameter>())
			{
				validTypeResolveParameters.Add(CreateInvertedTypeResolveParameter(typeResolveParameter, Type));
			}

			// Each of these sub-type type resolve attributes need to be checked and converted to a suitable structure to work with by the processors.
			foreach ((Type, Attribute) subTypeResolveAttribute in TypeReflectionUtilities.FindAllTypeDefinedAttributesInSubTypes(Type, attributeType, true))
			{
				if (!(subTypeResolveAttribute.Item2 is ITypeResolveParameter typeResolveParameter) || !typeResolveParameter.Target.IsAssignableFrom(Type))
				{
					continue;
				}
				
				validTypeResolveParameters.Add(CreateInvertedTypeResolveParameter(typeResolveParameter, subTypeResolveAttribute.Item1));
			}

			return typeResolveParameters.GetOrAdd(attributeType, validTypeResolveParameters.ToArray());

			ITypeResolveParameter CreateInvertedTypeResolveParameter(ITypeResolveParameter original, Type declaredType)
			{
				switch (original)
				{
					case ILookupTypeResolveParameter lookupParameter:
						return new InvertedLookupTypeResolveParameter()
						{
							Target = declaredType,
							Value = lookupParameter.Value,
							KeyOverride = lookupParameter.KeyOverride,
							OriginalAttribute = lookupParameter
						};
					default:
						throw new NotImplementedException(original.GetType().Name);
				}
			}
		}

		private ISerializableMember[] FindSerializableMembers(Type attributeType)
		{
			if (serializableMembers.TryGetValue(attributeType, out ISerializableMember[] result))
			{
				return result;
			}

			// Go over the fields and properties that have the desired attribute defined.
			List<ISerializableMember> serializableMembersForAttr = new List<ISerializableMember>();
			IEnumerable<MemberInfo> membersWithAttr = TypeReflectionUtilities.FindAllMembersWithAttribute(Type, attributeType, false, (MemberTypes.Field | MemberTypes.Property));
			membersWithAttr = TypeReflectionUtilities.FilterBaseMethods(membersWithAttr); // This filters out virtual/abstract properties with more concrete implementations.

			foreach (MemberInfo member in membersWithAttr)
			{
				switch (member.MemberType)
				{
					case MemberTypes.Field:
						Array.ForEach(
							Attribute.GetCustomAttributes(member, attributeType, true),
							(a) => serializableMembersForAttr.Add(new SerializableField(member as FieldInfo, a)));
						break;
					case MemberTypes.Property:
						Array.ForEach(
							Attribute.GetCustomAttributes(member, attributeType, true),
							(a) => serializableMembersForAttr.Add(new SerializableProperty(member as PropertyInfo, a)));
						break;
					default:
						throw new NotSupportedException(member.MemberType.DisplayName());
				}
			}

			return serializableMembers.GetOrAdd(attributeType, !serializableMembersForAttr.IsNullOrEmpty() ?
				serializableMembersForAttr.ToArray() :
				Array.Empty<ISerializableMember>());
		}

		private IRequiredSerializableMember[] FindRequiredMembers(Type attributeType)
		{
			if (requiredMembers.TryGetValue(attributeType, out IRequiredSerializableMember[] result))
			{
				return result;
			}

			// Go over the fields and properties that have the desired attribute defined.
			List<IRequiredSerializableMember> requiredMembersForAttr = new List<IRequiredSerializableMember>();
			foreach (MemberInfo member in TypeReflectionUtilities.FindAllMembersWithAttribute(Type, attributeType, false, (MemberTypes.Field | MemberTypes.Property)))
			{
				Array.ForEach(
					Attribute.GetCustomAttributes(member, attributeType, true),
					(a) => requiredMembersForAttr.Add(new RequiredSerializableMember(member, a)));
			}

			return requiredMembers.GetOrAdd(attributeType, !requiredMembersForAttr.IsNullOrEmpty() ?
				requiredMembersForAttr.ToArray() :
				Array.Empty<IRequiredSerializableMember>());
		}

		private ISerializationCallback[] FindSerializationCallbackMethods(Type attributeType)
		{
			if (serializationCallbacks.TryGetValue(attributeType, out ISerializationCallback[] result))
			{
				return result;
			}

			// Go over the methods that have the desired attribute defined.
			IEnumerable<MemberInfo> callbackMethods = TypeReflectionUtilities.FindAllMembersWithAttribute(Type, attributeType, false, MemberTypes.Method);
			List<ISerializationCallback> serializationCallbacksForAttr = new List<ISerializationCallback>();
			foreach (MemberInfo member in TypeReflectionUtilities.FilterBaseMethods(callbackMethods))
			{
				if (member is MethodInfo method)
				{
					Array.ForEach(
						Attribute.GetCustomAttributes(method, attributeType, true),
						(a) => serializationCallbacksForAttr.Add(new SerializationCallbackMethod(method, a)));
				}
			}

			return serializationCallbacks.GetOrAdd(attributeType, !serializationCallbacksForAttr.IsNullOrEmpty() ?
				serializationCallbacksForAttr.ToArray() :
				Array.Empty<ISerializationCallback>());
		}
	}
}