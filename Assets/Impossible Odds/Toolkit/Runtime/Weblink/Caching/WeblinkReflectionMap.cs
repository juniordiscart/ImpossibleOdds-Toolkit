namespace ImpossibleOdds.Weblink.Caching
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Concurrent;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public class WeblinkReflectionMap : IWeblinkReflectionMap
	{
		private readonly Type type = null;

		private ConcurrentDictionary<Type, Attribute[]> typeDefinedAttributes = new ConcurrentDictionary<Type, Attribute[]>();
		private ConcurrentDictionary<Type, ITargetedCallback[]> targetedCallbacks = new ConcurrentDictionary<Type, ITargetedCallback[]>();

		public WeblinkReflectionMap(Type type)
		{
			type.ThrowIfNull(nameof(type));
			this.type = type;
		}

		/// <inheritdoc />
		public Type Type
		{
			get => type;
		}

		/// <inheritdoc />
		public ITargetedCallback[] GetTargetedCallbacks(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return FindTargetedCallbacks(attributeType);
		}

		/// <inheritdoc />
		public bool IsAttributeDefinedOnType(Type attributeType)
		{
			attributeType.ThrowIfNull(nameof(attributeType));
			return !FindTypeDefinedAttributes(attributeType).IsNullOrEmpty();
		}

		private Attribute[] FindTypeDefinedAttributes(Type attributeType)
		{
			return typeDefinedAttributes.GetOrAdd(attributeType, (_) => Attribute.GetCustomAttributes(type, attributeType, true));
		}

		private ITargetedCallback[] FindTargetedCallbacks(Type attributeType)
		{
			if (targetedCallbacks.TryGetValue(attributeType, out ITargetedCallback[] callbacks))
			{
				return callbacks;
			}

			IEnumerable<MemberInfo> callbackMethods = TypeReflectionUtilities.FindAllMembersWithAttribute(type, attributeType, false, MemberTypes.Method);
			List<ITargetedCallback> targetedCallbacksForAttr = new List<ITargetedCallback>();
			foreach (MethodInfo method in TypeReflectionUtilities.FilterBaseMethods(callbackMethods))
			{
				Array.ForEach(
					Attribute.GetCustomAttributes(method, attributeType, true),
					(a) => targetedCallbacksForAttr.Add(new WeblinkTargetedCallbackMethod(method, a)));
			}

			return targetedCallbacks.GetOrAdd(attributeType, !targetedCallbacksForAttr.IsNullOrEmpty() ? targetedCallbacksForAttr.ToArray() : Array.Empty<ITargetedCallback>());
		}

		private TAttribute GetAttributeOnMember<TAttribute>(MemberInfo member, Type attributeType, TAttribute[] members)
		where TAttribute : IMemberAttributePair
		{
			return
				!members.IsNullOrEmpty() ?
				Array.Find(members, m => (m.Member == member) && (m.AttributeType == attributeType)) :
				default(TAttribute);
		}
	}
}
