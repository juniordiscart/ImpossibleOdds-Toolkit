namespace ImpossibleOdds.ReflectionCaching
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class TypeReflectionUtilities
	{
		/// <summary>
		/// Default: instance, public, and non-public.
		/// </summary>
		public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// Default: fields, properties and methods.
		/// </summary>
		public const MemberTypes DefaultMemberTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;

		private readonly static Type ObjectType = typeof(object);
		private readonly static ConcurrentDictionary<int, ConcurrentBag<object[]>> callbackParameterPool = new ConcurrentDictionary<int, ConcurrentBag<object[]>>();

		/// <summary>
		/// Finds the attributes of the given type defined on the target type. Optionally also includes those defined on the implemented interfaces.
		/// </summary>
		/// <param name="targetType">The type on which to search for attributes.</param>
		/// <param name="attributeType">The attribute that should be present on the members of the type.</param>
		/// <param name="includeInterfaces">Should implemented interfaces also be searched for the attribute?</param>
		/// <returns>The set of attributes defined on the type and its implemented interfaces.</returns>
		public static IEnumerable<Attribute> FindAllTypeDefinedAttributes(Type targetType, Type attributeType, bool includeInterfaces)
		{
			IEnumerable<Attribute> typeDefinedAttributes = Attribute.GetCustomAttributes(targetType, attributeType, true);

			if (includeInterfaces)
			{
				foreach (Type iType in targetType.GetInterfaces())
				{
					typeDefinedAttributes = typeDefinedAttributes.Union(Attribute.GetCustomAttributes(iType, attributeType, true));
				}
			}

			return typeDefinedAttributes;
		}

		/// <summary>
		/// Finds all members with the given attribute across the type chain.
		/// </summary>
		/// <param name="targetType">The type on which to search for members.</param>
		/// <param name="attributeType">The attribute that should be present on the members of the type.</param>
		/// <param name="includeInterfaces">Include members found </param>
		/// <param name="memberFilter">What members should be included in the result.</param>
		/// <param name="bindingFlags">What binding flags should be used for the members.</param>
		/// <returns>The set of members that have the attribute defined.</returns>
		public static IEnumerable<MemberInfo> FindAllMembersWithAttribute(Type targetType, Type attributeType, bool includeInterfaces, MemberTypes memberFilter = DefaultMemberTypes, BindingFlags bindingFlags = DefaultBindingFlags)
		{
			IEnumerable<MemberInfo> membersWithAttribute = Enumerable.Empty<MemberInfo>();

			Type itType = targetType;
			while ((itType != ObjectType) && (itType != null))
			{
				membersWithAttribute = membersWithAttribute
					.Union(
						itType
						.GetMembers(bindingFlags | BindingFlags.DeclaredOnly)   // Because we're going down the type chain anyway, we only get what is defined on this type.
						.Where(m => memberFilter.HasMemberFlag(m.MemberType) && Attribute.IsDefined(m, attributeType, true)));

				itType = itType.BaseType;
			}

			if (includeInterfaces)
			{
				foreach (Type iType in targetType.GetInterfaces())
				{
					membersWithAttribute = membersWithAttribute
						.Union(
							iType
							.GetMembers(bindingFlags)
							.Where(m => memberFilter.HasMemberFlag(m.MemberType) && Attribute.IsDefined(m, attributeType, true)));
				}
			}

			return membersWithAttribute;
		}

		/// <summary>
		/// Get a parameter list to invoke a method.
		/// </summary>
		/// <param name="length">The length of the parameter list to retrieve.</param>
		/// <returns>An array with the required amount of slots to invoke a method.</returns>
		public static object[] GetParameterInvokationList(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("A set of parameters must either larger than or equal to zero. Length of value {0} given.", length));
			}

			// Create a new list of parameters if no set is available.
			if (callbackParameterPool.TryGetValue(length, out ConcurrentBag<object[]> bag) && bag.TryTake(out object[] parameters))
			{
				return parameters;
			}
			else
			{
				return new object[length];
			}
		}

		/// <summary>
		/// Return a parameter list to the pool of parameters so that it may be reused.
		/// Note: the parameter list will be cleared of values.
		/// </summary>
		/// <param name="parameterList">The parameter list to return to the pool.</param>
		public static void ReturnParameterInvokationList(object[] parameterList)
		{
			parameterList.ThrowIfNull(nameof(parameterList));

			// Clear the parameter list first.
			for (int i = 0; i < parameterList.Length; ++i)
			{
				parameterList[i] = null;
			}

			callbackParameterPool.GetOrAdd(parameterList.Length, (_) => new ConcurrentBag<object[]>()).Add(parameterList);
		}

		/// <summary>
		/// Filters out multiple entries that refer to the same method in the virtual table. This applies to methods, properties and events.
		/// Only the most concrete implementation is kept in the final result set.
		/// </summary>
		/// <param name="members">The members to filter out.</param>
		/// <returns>A filtered list of members.</returns>
		public static IEnumerable<MemberInfo> FilterBaseMethods(IEnumerable<MemberInfo> members)
		{
			HashSet<MemberInfo> duplicateMembers = new HashSet<MemberInfo>();

			foreach (MemberInfo member in members)
			{
				switch (member.MemberType)
				{
					case MemberTypes.Method:
						MethodInfo m0 = member as MethodInfo;
						foreach (MethodInfo m1 in members.Where(m => m is MethodInfo))
						{
							// If the declaring type of m0 is a base of declaring type of m1, and m0 is a base definition of m1, then it is a duplicate.
							if ((m0 == m1) || !m0.DeclaringType.IsAssignableFrom(m1.DeclaringType))
							{
								continue;
							}

							if (IsMethodDuplicate(m0, m1))
							{
								duplicateMembers.Add(m0);
							}
						}
						break;
					case MemberTypes.Property:
						PropertyInfo p0 = member as PropertyInfo;
						foreach (PropertyInfo p1 in members.Where(m => m is PropertyInfo))
						{
							// Same as for methods, but based on the properties' backing methods.
							if ((p0 == p1) || !p0.DeclaringType.IsAssignableFrom(p1.DeclaringType))
							{
								continue;
							}

							if (IsPropertyDuplicate(p0, p1))
							{
								duplicateMembers.Add(p0);
							}
						}
						break;
					case MemberTypes.Event:
						EventInfo e0 = member as EventInfo;
						foreach (EventInfo e1 in members.Where(m => m is EventInfo))
						{
							if ((e0 == e1) || !e0.DeclaringType.IsAssignableFrom(e1.DeclaringType))
							{
								continue;
							}

							// Same as for methods, but based on the events' backing-methods.
							if (IsEventDuplicate(e0, e1))
							{
								duplicateMembers.Add(e0);
							}
						}
						break;
				}
			}

			return members.Except(duplicateMembers);
		}

		public static bool IsPropertyDuplicate(PropertyInfo p0, PropertyInfo p1)
		{
			p0.ThrowIfNull(nameof(p0));
			p1.ThrowIfNull(nameof(p1));

			bool gettersDuplicate = false;
			if (p0.CanRead == p1.CanRead)
			{
				if (p0.CanRead && p1.CanRead)
				{
					gettersDuplicate = p0.GetMethod.IsVirtual && p1.GetMethod.IsVirtual && (p0.GetMethod.GetBaseDefinition() == p1.GetMethod.GetBaseDefinition());
				}
				else
				{
					gettersDuplicate = !p0.CanRead && !p1.CanRead;
				}
			}

			bool settersDuplicate = false;
			if (p0.CanWrite == p1.CanWrite)
			{
				if (p0.CanWrite && p1.CanWrite)
				{
					settersDuplicate = p0.SetMethod.IsVirtual && p1.SetMethod.IsVirtual && (p0.SetMethod.GetBaseDefinition() == p1.SetMethod.GetBaseDefinition());
				}
				else
				{
					settersDuplicate = !p0.CanWrite && !p1.CanWrite;
				}
			}

			return gettersDuplicate && settersDuplicate;
		}

		public static bool IsEventDuplicate(EventInfo e0, EventInfo e1)
		{
			e0.ThrowIfNull(nameof(e0));
			e1.ThrowIfNull(nameof(e1));

			bool addersDuplicate = e0.AddMethod.IsVirtual && e1.AddMethod.IsVirtual && (e0.AddMethod.GetBaseDefinition() == e1.AddMethod.GetBaseDefinition());
			bool removersDuplicate = e0.RemoveMethod.IsVirtual && e1.RemoveMethod.IsVirtual && (e0.RemoveMethod.GetBaseDefinition() == e1.RemoveMethod.GetBaseDefinition());

			return addersDuplicate && removersDuplicate;
		}

		public static bool IsMethodDuplicate(MethodInfo m0, MethodInfo m1)
		{
			m0.ThrowIfNull(nameof(m0));
			m1.ThrowIfNull(nameof(m1));
			return m0.IsVirtual && m1.IsVirtual && (m0.GetBaseDefinition() == m1.GetBaseDefinition());
		}

		/// <summary>
		/// Explicit implementation to prevent a memory alloc.
		/// </summary>
		/// <param name="m">The enum member value.</param>
		/// <param name="flagToTest">The flag(s) to test whether they are present in the value or not.</param>
		/// <returns>True if present, false otherwise.</returns>
		private static bool HasMemberFlag(this MemberTypes m, MemberTypes flagToTest)
		{
			return (m & flagToTest) == flagToTest;
		}
	}
}
