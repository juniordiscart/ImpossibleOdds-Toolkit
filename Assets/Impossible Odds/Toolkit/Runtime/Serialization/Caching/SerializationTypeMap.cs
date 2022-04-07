namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class SerializationTypeMap
	{
		private readonly Type type = null;
		private Dictionary<Type, IReadOnlyList<IMemberAttributeTuple>> membersWithAttributes = new Dictionary<Type, IReadOnlyList<IMemberAttributeTuple>>();
		private Dictionary<Type, IReadOnlyList<ITypeResolveParameter>> typeResolveParameters = new Dictionary<Type, IReadOnlyList<ITypeResolveParameter>>();
		private Dictionary<Type, IReadOnlyList<SerializationCallbackInfo>> serializationCallbacks = new Dictionary<Type, IReadOnlyList<SerializationCallbackInfo>>();
		private Dictionary<Type, IReadOnlyList<IRequiredMemberAttributeTuple>> requiredMembers = new Dictionary<Type, IReadOnlyList<IRequiredMemberAttributeTuple>>();

		public Type Type
		{
			get { return type; }
		}

		public SerializationTypeMap(Type type)
		{
			type.ThrowIfNull(nameof(type));
			this.type = type;
		}

		/// <summary>
		/// Retrieve all members with a specific attribute defined.
		/// </summary>
		/// <param name="typeOfAttribute">Type of the attribute that will be looked for on members.</param>
		/// <returns>A collection of members that have the attribute defined on them.</returns>
		public IReadOnlyList<IMemberAttributeTuple> GetMembersWithAttribute(Type typeOfAttribute)
		{
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (membersWithAttributes.ContainsKey(typeOfAttribute))
			{
				return membersWithAttributes[typeOfAttribute];
			}

			// Collection in which we will store the cached fields.
			List<IMemberAttributeTuple> targetMembers = new List<IMemberAttributeTuple>();

			// Fetch all fields across the type hierarchy.
			Type targetType = Type;
			while ((targetType != null) && (targetType != typeof(object)))
			{
				MemberInfo[] members = targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MemberInfo member in members)
				{
					if (member.DeclaringType != targetType)
					{
						continue;
					}

					Attribute attr = member.GetCustomAttribute(typeOfAttribute, false);
					if (attr == null)
					{
						continue;
					}

					if (member is FieldInfo field)
					{
						targetMembers.Add(new FieldAttributeTuple(field, attr));
					}
					else if (member is PropertyInfo property)
					{
						targetMembers.Add(new PropertyAttributeTuple(property, attr));
					}
				}

				targetType = targetType.BaseType;
			}

			membersWithAttributes[typeOfAttribute] = targetMembers;
			return targetMembers;
		}

		/// <summary>
		/// Retrieve all type resolve parameters defined on a type of a specific type resolve attribute type.
		/// </summary>
		/// <param name="typeOfAttribute">The type resolve parameter attribute type.</param>
		/// <returns>A collection of all type resolve parameters defined on the type and it's inheritance tree.</returns>
		public IReadOnlyList<ITypeResolveParameter> GetTypeResolveParameters(Type typeOfAttribute)
		{
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (!typeof(ITypeResolveParameter).IsAssignableFrom(typeOfAttribute))
			{
				throw new Serialization.SerializationException("The requested attribute type to look for does not implement the {0} interface.", typeof(ITypeResolveParameter).Name);
			}

			if (typeResolveParameters.ContainsKey(typeOfAttribute))
			{
				return typeResolveParameters[typeOfAttribute];
			}

			// Fetch all attributes defined in the inheritance chain.
			List<ITypeResolveParameter> typeResolveAttributes = new List<ITypeResolveParameter>();

			// Attributes defined on the class itself.
			// Filter out those that aren't assignable to the current type.
			typeResolveAttributes.AddRange(Type.GetCustomAttributes(typeOfAttribute, true).Cast<ITypeResolveParameter>().Where(t => Type.IsAssignableFrom(t.Target)));

			// Attributes defined in interfaces implemented by the class or one of it's base classes.
			// Filter out those that aren't assignable to the current type.
			foreach (Type interfaceType in Type.GetInterfaces())
			{
				typeResolveAttributes.AddRange(interfaceType.GetCustomAttributes(typeOfAttribute, true).Cast<ITypeResolveParameter>().Where(t => Type.IsAssignableFrom(t.Target)));
			}

			typeResolveParameters[typeOfAttribute] = typeResolveAttributes;
			return typeResolveAttributes;
		}

		/// <summary>
		/// Retrieve all callback methods defined for a specific callback attribute type.
		/// </summary>
		/// <param name="typeOfAttribute">Type of the callback attribute.</param>
		/// <returns>A collection of all methods that are defined as a callback.</returns>
		public IReadOnlyList<SerializationCallbackInfo> GetSerializationCallbacks(Type typeOfAttribute)
		{
			typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

			if (serializationCallbacks.ContainsKey(typeOfAttribute))
			{
				return serializationCallbacks[typeOfAttribute];
			}

			List<SerializationCallbackInfo> callbackMethods = new List<SerializationCallbackInfo>();

			Type currentType = Type;
			BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			while ((currentType != null) && (currentType != typeof(object)))
			{
				foreach (MethodInfo callbackMethod in currentType.GetMethods(methodFlags).Where(m => m.IsDefined(typeOfAttribute)))
				{
					callbackMethods.Add(new SerializationCallbackInfo(callbackMethod));
				}

				currentType = currentType.BaseType;
			}

			serializationCallbacks[typeOfAttribute] = callbackMethods;
			return callbackMethods;
		}

		/// <summary>
		/// Is the member required when being processed using the provided serialziation definition?
		/// </summary>
		/// <param name="memberInfo">The member to check for whether it is required.</param>
		/// <param name="requiredValueSupport">The serialization definition containing information which attribute is used to mark members as required.</param>
		/// <returns>True, if the member is required. False otherwise.</returns>
		public bool IsMemberRequired(MemberInfo memberInfo, IRequiredValueSupport requiredValueSupport)
		{
			requiredValueSupport.ThrowIfNull(nameof(requiredValueSupport));
			return IsMemberRequired(memberInfo, requiredValueSupport.RequiredAttributeType);
		}

		public bool IsMemberRequired(MemberInfo memberInfo, IRequiredValueSupport requiredValueSupport, out IRequiredMemberAttributeTuple requiredMemberInfo)
		{
			requiredValueSupport.ThrowIfNull(nameof(requiredValueSupport));
			return IsMemberRequired(memberInfo, requiredValueSupport.RequiredAttributeType, out requiredMemberInfo);
		}

		/// <summary>
		/// Is the member considered a required parameter?
		/// </summary>
		/// <param name="memberInfo">The member to check for whether it is required.</param>
		/// <param name="typeOfRequiredParameter">The type of attribute to check for on the member.</param>
		/// <returns>True, if the member is required. False otherwise.</returns>
		public bool IsMemberRequired(MemberInfo memberInfo, Type typeOfRequiredParameter)
		{
			memberInfo.ThrowIfNull(nameof(memberInfo));
			return GetRequiredMembers(typeOfRequiredParameter).Any(m => (m.Member == memberInfo));
		}

		public bool IsMemberRequired(MemberInfo memberInfo, Type typeOfRequiredParameter, out IRequiredMemberAttributeTuple requiredMemberInfo)
		{
			memberInfo.ThrowIfNull(nameof(memberInfo));
			IReadOnlyList<IRequiredMemberAttributeTuple> requiredMembers = GetRequiredMembers(typeOfRequiredParameter);

			if (requiredMembers.Any(rm => rm.Member == memberInfo))
			{
				requiredMemberInfo = requiredMembers.First(rm => rm.Member == memberInfo);
				return true;
			}
			else
			{
				requiredMemberInfo = null;
				return false;
			}
		}

		/// <summary>
		/// Retrieve all members
		/// </summary>
		/// <param name="typeOfRequiredParameter"></param>
		/// <returns></returns>
		public IReadOnlyList<IRequiredMemberAttributeTuple> GetRequiredMembers(Type typeOfRequiredParameter)
		{
			typeOfRequiredParameter.ThrowIfNull(nameof(typeOfRequiredParameter));

			if (requiredMembers.ContainsKey(typeOfRequiredParameter))
			{
				return requiredMembers[typeOfRequiredParameter];
			}

			// Collection in which we will store the cached fields.
			List<IRequiredMemberAttributeTuple> targetMembers = new List<IRequiredMemberAttributeTuple>();

			// Fetch all fields across the type hierarchy.
			Type targetType = Type;
			while ((targetType != null) && (targetType != typeof(object)))
			{
				MemberInfo[] members = targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MemberInfo member in members)
				{
					if (member.DeclaringType != targetType)
					{
						continue;
					}

					Attribute attr = member.GetCustomAttribute(typeOfRequiredParameter, false);
					if (attr != null)
					{
						targetMembers.Add(new RequiredMemberAttributeTuple(member, attr as IRequiredParameter));
					}
				}

				targetType = targetType.BaseType;
			}

			requiredMembers[typeOfRequiredParameter] = targetMembers;
			return targetMembers;
		}
	}
}
