using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Weblink.Caching
{
	public class WeblinkTargetedCallbackMethod : ITargetedCallback
	{
		public WeblinkTargetedCallbackMethod(MethodInfo callbackMethod, Attribute callbackAttribute)
		{
			callbackMethod.ThrowIfNull(nameof(callbackMethod));
			callbackAttribute.ThrowIfNull(nameof(callbackAttribute));

			if (callbackAttribute is IWeblinkResponseTypeAssociation responseAssociation)
			{
				ResponseType = responseAssociation.ResponseType;
			}
			else
			{
				throw new ReflectionCachingException("The attribute defining a targeted callback does not implement the {0} interface.", callbackAttribute.GetType().Name);
			}

			Method = callbackMethod;
			Attribute = callbackAttribute;
			Parameters = callbackMethod.GetParameters();
		}

		/// <inheritdoc />
		public MethodInfo Method { get; }

		/// <inheritdoc />
		public ParameterInfo[] Parameters { get; }

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => Method;

		/// <inheritdoc />
		public Attribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => Attribute.GetType();

		/// <inheritdoc />
		public Type ResponseType { get; }
	}
}