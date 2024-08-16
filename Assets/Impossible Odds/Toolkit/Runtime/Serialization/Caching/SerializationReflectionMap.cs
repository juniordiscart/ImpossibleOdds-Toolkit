using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
	/// <summary>
	/// An abstract reflection map for serialization purposes.
	/// </summary>
	public class SerializationReflectionMap : ISerializationReflectionMap
	{
		private readonly ConcurrentDictionary<Type, Attribute[]> typeDefinedAttributes = new ConcurrentDictionary<Type, Attribute[]>();
		private readonly ConcurrentDictionary<Type, ITypeResolutionParameter[]> typeResolutionParameters = new ConcurrentDictionary<Type, ITypeResolutionParameter[]>();
		private readonly ConcurrentDictionary<Type, ISerializableMember[]> serializableMembers = new ConcurrentDictionary<Type, ISerializableMember[]>();
		private readonly ConcurrentDictionary<Type, ISerializableMember[]> uniqueSerializableMembers = new ConcurrentDictionary<Type, ISerializableMember[]>();
		private readonly ConcurrentDictionary<Type, ISerializationCallback[]> serializationCallbacks = new ConcurrentDictionary<Type, ISerializationCallback[]>();
		private readonly ConcurrentDictionary<Type, IRequiredSerializableMember[]> requiredMembers = new ConcurrentDictionary<Type, IRequiredSerializableMember[]>();

		public SerializationReflectionMap(Type type)
		{
			type.ThrowIfNull(nameof(type));
			Type = type;
		}

		/// <inheritdoc />
		public Type Type { get; }

		/// <inheritdoc />
		public ITypeResolutionParameter[] GetTypeResolutionParameters(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindTypeResolutionParameters(attributeType);
		}

		/// <inheritdoc />
		public ISerializableMember[] GetSerializableMembers(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindSerializableMembers(attributeType);
		}

		/// <inheritdoc />
		public ISerializableMember[] GetUniqueSerializableMembers(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindUniqueSerializableMembers(attributeType);
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

		private ITypeResolutionParameter[] FindTypeResolutionParameters(Type attributeType)
		{
			if (typeResolutionParameters.TryGetValue(attributeType, out ITypeResolutionParameter[] result))
			{
				return result;
			}
			
			List<ITypeResolutionParameter> validTypeResolutionParameters = new List<ITypeResolutionParameter>();
			AddValidTypeResolutionParameters(validTypeResolutionParameters, TypeReflectionUtilities.FindAllTypeDefinedAttributes(Type, attributeType, true));
			AddValidTypeResolutionParameters(validTypeResolutionParameters, TypeReflectionUtilities.FindAllTypeDefinedAttributesInSubTypes(Type, attributeType, true));

			return typeResolutionParameters.GetOrAdd(attributeType, validTypeResolutionParameters.ToArray());

			void AddValidTypeResolutionParameters(List<ITypeResolutionParameter> validParameters, IEnumerable<(Type, Attribute)> attributes)
			{
				foreach ((Type, Attribute) typeDefinedAttribute in attributes)
				{
					ITypeResolutionParameter typeResolutionParameter = (ITypeResolutionParameter)typeDefinedAttribute.Item2;
				
					// If the type is assignable from the type resolution target.
					if (Type.IsAssignableFrom(typeResolutionParameter.Target) || typeDefinedAttribute.Item1.IsAssignableFrom(typeResolutionParameter.Target))
					{
						validParameters.Add(typeResolutionParameter);
						continue;
					}

					// If the type resolution  target is assignable from the type.
					if (typeResolutionParameter.Target.IsAssignableFrom(Type))
					{
						validParameters.Add(typeResolutionParameter switch
						{
							ILookupTypeResolutionParameter lookupParameter => new InvertedLookupTypeResolutionParameter(typeDefinedAttribute.Item1, lookupParameter),
							ISequenceTypeResolutionParameter sequenceParameter => new InvertedSequenceTypeResolutionParameter(typeDefinedAttribute.Item1, sequenceParameter),
							_ => new InvertedTypeResolutionParameter(typeDefinedAttribute.Item1, typeResolutionParameter)
						});
						continue;
					}
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
			membersWithAttr = TypeReflectionUtilities.FilterBaseMethods(membersWithAttr);   // This filters out virtual/abstract properties with more concrete implementations.

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
				}
			}

			return serializableMembers.GetOrAdd(attributeType, !serializableMembersForAttr.IsNullOrEmpty() ? serializableMembersForAttr.ToArray() : Array.Empty<ISerializableMember>());
		}

		private ISerializableMember[] FindUniqueSerializableMembers(Type attributeType)
		{
			ISerializableMember[] allSerializableMembers = FindSerializableMembers(attributeType);
			List<ISerializableMember> filteredCache = new List<ISerializableMember>(allSerializableMembers.Length);

			foreach (ISerializableMember serializableMember in allSerializableMembers)
			{
				if (filteredCache.TryFindIndex(cachedMember => SerializationUtilities.IsMatch(cachedMember, serializableMember), out int index))
				{
					if (serializableMember.Member.DeclaringType.IsSubclassOf(filteredCache[index].Member.DeclaringType))
					{
						filteredCache[index] = serializableMember;
					}
				}
				else
				{
					filteredCache.Add(serializableMember);
				}
			}

			return uniqueSerializableMembers.GetOrAdd(attributeType, !filteredCache.IsNullOrEmpty() ? filteredCache.ToArray() : Array.Empty<ISerializableMember>());
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

			return requiredMembers.GetOrAdd(attributeType, !requiredMembersForAttr.IsNullOrEmpty() ? requiredMembersForAttr.ToArray() : Array.Empty<IRequiredSerializableMember>());
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
			foreach (MethodInfo method in TypeReflectionUtilities.FilterBaseMethods(callbackMethods).Cast<MethodInfo>())
			{
				Array.ForEach(
					Attribute.GetCustomAttributes(method, attributeType, true),
					(a) => serializationCallbacksForAttr.Add(new SerializationCallbackMethod(method, a)));
			}

			return serializationCallbacks.GetOrAdd(attributeType, !serializationCallbacksForAttr.IsNullOrEmpty() ? serializationCallbacksForAttr.ToArray() : Array.Empty<ISerializationCallback>());
		}
	}
}